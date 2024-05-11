namespace MusicHub
{
    using System;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here


            var songs = ExportSongsAboveDuration(context, 4);
            Console.WriteLine(songs);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            var filteredAlbums = context.Albums
                .AsNoTracking()
                .Include(a => a.Producer)
                .Include(a => a.Songs)
                .ThenInclude(s => s.Writer)
                .Where(a => a.ProducerId == producerId)
                .ToList()
                .Select(a => new
                {
                    a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                    ProducerName = a!.Producer!.Name,
                    Songs = a.Songs.Select(s => new
                    {
                        s.Name,
                        Price = s.Price.ToString("f2"),
                        WriterName = s.Writer.Name
                    })
                    .OrderByDescending(s => s.Name)
                    .ThenBy(s => s.WriterName)
                    .ToList(),
                    AlbumPrice = a.Price
                })
                .OrderByDescending(s => s.AlbumPrice)
                .ToList();

            foreach (var album in filteredAlbums)
            {
                sb.AppendLine($"-AlbumName: {album.Name}")
                  .AppendLine($"-ReleaseDate: {album.ReleaseDate}")
                  .AppendLine($"-ProducerName: {album.ProducerName}");

                if(album.Songs.Count > 0)
                {
                   sb.AppendLine("-Songs:");
                }
                

                int count = 1;

                foreach (var song in album.Songs)
                {

                    sb
                        .AppendLine($"---#{count}")
                        .AppendLine($"---SongName: {song.Name}")
                        .AppendLine($"---Price: {song.Price}")
                        .AppendLine($"---Writer: {song.WriterName}");

                    count++;
                }


                sb.AppendLine($"-AlbumPrice: {album.AlbumPrice:f2}");
            }


            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var filteredSongs = context.Songs
                .AsNoTracking()
                .Include(s => s.Album)
                .ThenInclude(a => a.Producer)
                .Include(s => s.Writer)
                .Include(s => s.SongPerformers)
                .ThenInclude(sp => sp.Performer)
                .ToList()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    s.Name,
                    Performers = s.SongPerformers
                           .Select(sp => sp.Performer.FirstName + ' ' + sp.Performer.LastName)
                           .OrderBy(sp => sp)
                           .ToList(),
                    WriterName = s.Writer.Name,
                    AlbumProducer = s!.Album!.Producer!.Name,
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.Name)
                .ThenBy(s => s.WriterName)
                .ToList();


            int count = 1;



            foreach (var song in filteredSongs)
            {
              

                sb
                    .AppendLine($"-Song #{count}")
                    .AppendLine($"---SongName: {song.Name}")
                    .AppendLine($"---Writer: {song.WriterName}");



                foreach (var performer in song.Performers)
                {
                    sb.AppendLine($"---Performer: {performer}");
                }


                sb
                    .AppendLine($"---AlbumProducer: {song.AlbumProducer}")
                    .AppendLine($"---Duration: {song.Duration}");

                count++;
                     
            }


            return sb.ToString().TrimEnd();

        }
    }
}
