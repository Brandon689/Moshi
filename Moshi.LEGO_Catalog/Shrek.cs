using ImageMagick;
using BricklinkSharp.Client;
using Moshi.LEGO_Catalog.Services;
using build_lego_catalog.Models;

namespace Moshi.LEGO_Catalog;

public class Shrek
{
    IBricklinkClient client;
    CatalogService s;
    public Shrek()
    {
        s = new CatalogService("Data Source=LegoCatalog.db;Version=3;");
        //BricklinkClientConfiguration.Instance.TokenValue = "69F6EB5074D84A32969CB4A3B174BAFC";
        //BricklinkClientConfiguration.Instance.TokenSecret = "DB8E817D458D420497D90CF45F2B89FA";
        //BricklinkClientConfiguration.Instance.ConsumerKey = "C45212094D004DEDBAF175A2DF0B9398";
        //BricklinkClientConfiguration.Instance.ConsumerSecret = "D3C6D97549CC4842986506CCE06B51F4";
        //client = BricklinkClientFactory.Build();
    }
    public async Task Moon()
    {
        //var itemMappings = await client.GetElementIdAsync("3003", 0);
       
        var free = await s.GetAllCodesAsync(0, 99999999);
        var rainbow = await s.GetAllColorsAsync(0, 9999999);
        var sets = await s.GetAllBooksAsync(0, 99999999);
        ;
        //List<ColorEx> ols = new();
        //foreach (var item in free)
        //{
        //    ColorEx color = new ColorEx();
        //    var match = rainbow.FirstOrDefault(x => x.ColorName == item.Color);
        //    color.ColorId = match.ColorID;
        //    color.ItemNo = item.ItemNo;
        //    ols.Add(color);
        //}
        var d = sets.ToList();
        for (int i = 6000; i < d.Count; i++)
        {
            //    var knownColors = ols.Where(x => x.ItemNo == d[i].Number).ToArray();
            //    //var knownColors = await client.GetKnownColorsAsync(ItemType.Part, "3006");


            //    for (int j = 0; j < knownColors.Length; j++)
            //    {
            //        Directory.CreateDirectory($"parts/{d[i].Number}");
            //        string file = $"parts/{d[i].Number}/{d[i].Number}_{knownColors[j].ColorId}.avif";
            //        if (File.Exists(file)) continue;
            //        try
            //        {
            //            var img = $"https://img.bricklink.com/ItemImage/PN/{knownColors[j].ColorId}/{d[i].Number}.png";
            //            using (var image = new MagickImage(img))
            //            {
            //                // Set general quality (0-100, where 100 is the best quality)
            //                image.Quality = 50;

            //                // AVIF-specific settings
            //                //image.Settings.SetDefine("avif:speed", "0");  // Encoding speed (0-10, where 0 is slowest/best)
            //                //image.Settings.SetDefine("avif:compression", "av1");  // Use AV1 compression
            //                //image.Settings.SetDefine("avif:color-profile", "true");  // Preserve color profile

            //                // For more precise control over quality
            //                //image.Settings.SetDefine("avif:qmin", "0");  // Minimum quantizer (0-63, lower is better quality)
            //                //image.Settings.SetDefine("avif:qmax", "63");  // Maximum quantizer (0-63, lower is better quality)
            //                if (i % 10 == 0)
            //                    Console.WriteLine(i);
            //                // Write the image
            //                image.Write(file);
            //            }
            //        }
            //        catch
            //        {
            //            //var img = $"https://img.bricklink.com/ItemImage/PN/{knownColors[j].ColorId}/{d[i].Number}.png";
            //            //Console.WriteLine(img);
            //            //Console.WriteLine("error: " + d[i].Number);
            //        }

            string file = $"books2/{d[i].Number}.avif";
            if (File.Exists(file)) continue;
            try
            {
                var img = $"https://img.bricklink.com/ItemImage/BN/0/{d[i].Number}.jpg";
                using (var image = new MagickImage(img))
                {
                    // Set general quality (0-100, where 100 is the best quality)
                    image.Quality = 50;

                    // AVIF-specific settings
                    //image.Settings.SetDefine("avif:speed", "0");  // Encoding speed (0-10, where 0 is slowest/best)
                    //image.Settings.SetDefine("avif:compression", "av1");  // Use AV1 compression
                    //image.Settings.SetDefine("avif:color-profile", "true");  // Preserve color profile

                    // For more precise control over quality
                    //image.Settings.SetDefine("avif:qmin", "0");  // Minimum quantizer (0-63, lower is better quality)
                    //image.Settings.SetDefine("avif:qmax", "63");  // Maximum quantizer (0-63, lower is better quality)
                    Console.WriteLine(i);
                    // Write the image
                    image.Write(file);
                }
            }
            catch
            {
                Console.WriteLine(d[i].Number);
            }

        }


    
    }

    public class ColorEx : Code
    {
        public string ColorId { get; set; }
        public string ItemNo { get; set; }
    }
}
