using Microsoft.Data.SqlClient;
using System.Text;

namespace Ado.NET
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            await using SqlConnection connection = new SqlConnection(Config.ConnectionString);
            await connection.OpenAsync();

            //string result = await GetVillainNamesAsync(connection);
            //Console.WriteLine(result);

            //int villainId = int.Parse(Console.ReadLine());
            //string minionNamesResult = await GetMinionNamesByVillainIdAsync(connection, villainId);
            //Console.WriteLine(minionNamesResult);



            string[] minionTokens = Console.ReadLine().Split(' ');
            string[] villainTokens = Console.ReadLine().Split(' ');
            string minionName = minionTokens[1];
            int age = int.Parse(minionTokens[2]);
            string town = minionTokens[3];
            string villain = villainTokens[1];

            string addMinionResult = await AddMinionAsync(connection, minionName, age, town, villain);
            Console.WriteLine(addMinionResult);
        }

        static async Task<string> GetVillainNamesAsync(SqlConnection connection)
        {
            StringBuilder sb = new StringBuilder();

            SqlCommand getVillainsCmd = new SqlCommand(SqlQueries.GetVillainNamesQuery, connection);
            SqlDataReader reader = await getVillainsCmd.ExecuteReaderAsync();

            while(reader.Read())
            {
                string name = (string)reader["Name"];
                int minionsCount = (int)reader["MinionsCount"];

                sb.AppendLine($"{name} - {minionsCount}");
            }

            return sb.ToString().TrimEnd();
        }

        static async Task<string> GetMinionNamesByVillainIdAsync(SqlConnection connection, int id)
        {
            StringBuilder sb = new StringBuilder();

            SqlCommand getVillainCmd = new SqlCommand(SqlQueries.GetVillainNameByIdQuery, connection);
            getVillainCmd.Parameters.AddWithValue("@Id", id);

            string? villainName = (string?)(await getVillainCmd.ExecuteScalarAsync());

            if(villainName == null)
            {
                return $"No villain with ID {id} exists in the database.";
            }

            sb.AppendLine($"Villain: {villainName}");

            SqlCommand getMinionsCmd = new SqlCommand(SqlQueries.GetMinionsNames, connection);
            getMinionsCmd.Parameters.AddWithValue("@Id", id);

            SqlDataReader minionsResult = await getMinionsCmd.ExecuteReaderAsync();

            if (!minionsResult.HasRows)
            {
                sb.AppendLine("(no minions)");
            }
            else
            {
                int counter = 1;

                while (minionsResult.Read())
                {
                    string minionName = (string)minionsResult["Name"];
                    int minionAge = (int)minionsResult["Age"];

                    sb.AppendLine($"{counter++}. {minionName} {minionAge}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        static async Task<string> AddMinionAsync(SqlConnection connection, string minionName, int age, string town, string villainName)
        {
            StringBuilder sb = new StringBuilder();


            SqlCommand townExistsCmd = new SqlCommand(SqlQueries.GetTownByNameQuery, connection);
            townExistsCmd.Parameters.AddWithValue("@townName", town);

            int? townResult = (int?)(await townExistsCmd.ExecuteScalarAsync());

            if(townResult == null)
            {
                SqlCommand insertNewTownCmd = new SqlCommand(SqlQueries.AddNewTownQuery, connection);
                insertNewTownCmd.Parameters.AddWithValue("@townName", town);
                await insertNewTownCmd.ExecuteNonQueryAsync();

                sb.AppendLine($"Town {town} was added to the database.");
            }


            SqlCommand villainExistsCmd = new SqlCommand(SqlQueries.GetVillainByNameQuery, connection);
            villainExistsCmd.Parameters.AddWithValue("@Name", villainName);

            int? villainResult = (int?)(await villainExistsCmd.ExecuteScalarAsync());

            if(villainResult == null)
            {
                SqlCommand addNewVillainCmd = new SqlCommand(SqlQueries.AddNewVillainQuery, connection);
                addNewVillainCmd.Parameters.AddWithValue("@villainName", villainName);
                
                await addNewVillainCmd.ExecuteNonQueryAsync();
                sb.AppendLine($"Villain {villainName} was added to the database.");

            }


            SqlCommand addNewMinionCmd = new SqlCommand(SqlQueries.AddNewMinionQuery, connection);

            addNewMinionCmd.Parameters.AddWithValue("@name", minionName);
            addNewMinionCmd.Parameters.AddWithValue("@age", age);
            addNewMinionCmd.Parameters.AddWithValue("@townId", townResult);

            await addNewMinionCmd.ExecuteNonQueryAsync();


            SqlCommand minionExistsCmd = new SqlCommand(SqlQueries.GetMinionByNameQuery, connection);
            minionExistsCmd.Parameters.AddWithValue("@Name", minionName);
            int minionId = (int)await minionExistsCmd.ExecuteScalarAsync();


            SqlCommand minionServantOfVillainCmd = new SqlCommand(SqlQueries.AddNewMinionVillainQuery, connection);
            minionServantOfVillainCmd.Parameters.AddWithValue("@minionId", minionId);
            minionServantOfVillainCmd.Parameters.AddWithValue("@villainId", villainResult);

            sb.AppendLine($"Successfully added {minionName} to be minion of {villainName}.");

            return sb.ToString().TrimEnd();
        }
    }
}
