using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
namespace LeagueOfLegos
{
    public static class DDragon
    {
        private static FullConvertedChampData Roster;
        private static readonly string apiKey = "RGAPI-23b67449-056a-4254-9b4d-cb8f0d2ff057";
        public static StorageFolder Thumbs { get; set; }
        public static StorageFolder LoadingScr { get; set; }
        private static HttpClient wizard = new HttpClient();

        public static async Task<FullConvertedChampData> AskTheDragonAsync()
        {
            if (Roster != null)
                return Roster;
            var champDataStream = await wizard.GetStreamAsync(
                $"https://na.api.pvp.net/api/lol/static-data/oce/v1.2/champion?champData=image,skins&api_key={apiKey}");
            string jsonObj = new StreamReader(champDataStream).ReadToEnd();
            Roster = Newtonsoft.Json.JsonConvert.DeserializeObject<FullConvertedChampData>(jsonObj);

            // cache the values for later
            await GetThumbs();
            return Roster;
        }

        private static async Task GetImageData(StorageFolder saveFolder, string saveFileName, Uri remoteImageLocation)
        {
            var fileIfExists = await saveFolder.TryGetItemAsync(saveFileName);
            if (fileIfExists == null)
            {
                var loadingImg = await wizard.GetByteArrayAsync(remoteImageLocation);
                StorageFile targetFile = await saveFolder.CreateFileAsync(saveFileName, CreationCollisionOption.GenerateUniqueName);
                using (var imgStream = await targetFile.OpenStreamForWriteAsync())
                {
                    await imgStream.WriteAsync(loadingImg, 0, loadingImg.Length);
                    await imgStream.FlushAsync();
                }
            }
        }

        public static async Task<Uri> GetSplash(string champ, int skinId)
        {
            if (LoadingScr == null)
                LoadingScr = await ApplicationData.Current.LocalFolder.CreateFolderAsync(nameof(LoadingScr), CreationCollisionOption.OpenIfExists);
            string imgName = $"{champ}_{skinId}.jpg";
            Uri imgLocation = new Uri($"http://ddragon.leagueoflegends.com/cdn/img/champion/loading/{imgName}");
            await GetImageData(LoadingScr, imgName, imgLocation);
            return new Uri($"{LoadingScr.Path}/{imgName}");
        }

        private static async Task GetThumbs()
        {
            if (Thumbs == null)
                Thumbs = await ApplicationData.Current.LocalFolder.CreateFolderAsync(nameof(Thumbs), CreationCollisionOption.OpenIfExists);

            foreach (var nameChamp in Roster.data)
            {
                string imgName = nameChamp.Value.image.full;
                Uri imgLocation = new Uri($"http://ddragon.leagueoflegends.com/cdn/7.6.1/img/champion/{imgName}");
                await GetImageData(Thumbs, imgName, imgLocation);
            }
        }

        public static Uri GetThumbName(string imgName)
        {
            return new Uri($"{Thumbs.Path}/{imgName}");
        }
    }
}
