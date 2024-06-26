using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ado.NET
{
    public static class SqlQueries
    {
        public const string GetVillainNamesQuery =
          @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
           FROM Villains AS v 
            JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
        GROUP BY v.Id, v.Name 
          HAVING COUNT(mv.VillainId) > 3 
        ORDER BY COUNT(mv.VillainId)";


        public const string GetVillainNameByIdQuery =
            @"SELECT Name FROM Villains WHERE Id = @Id";


        public const string GetMinionsNames =
            @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) AS RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";

        public const string AddNewTownQuery =
            @"INSERT INTO Towns (Name) VALUES (@townName)";


        public const string AddNewMinionQuery =
            @"INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId)";

        public const string AddNewVillainQuery =
            @"INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";

        public const string AddNewMinionVillainQuery =
            @"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@minionId, @villainId)";

        public const string GetTownByNameQuery =
            @"SELECT Id FROM Towns WHERE Name = @townName";

        public const string GetMinionByNameQuery =
            @"SELECT Id FROM Minions WHERE Name = @Name";

        public const string GetVillainByNameQuery =
            @"SELECT Id FROM Villains WHERE Name = @Name";

    }


}
