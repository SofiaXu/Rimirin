using Rimirin.Models.Garupa;
using SkiaSharp;
using SkiaSharp.Extended.Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Garupa
{
    public class GachaImageRender : IDisposable
    {
        private readonly BestdoriClient client;
        private readonly GarupaData data;
        public GachaImageRender(BestdoriClient bestdori, GarupaData data)
        {
            this.client = bestdori;
            this.data = data;
        }
        public async Task<string> RenderGachaImageAsync(IList<(string, Card)> cards)
        {
            using SKBitmap bitmap = SKBitmap.Decode("Resources\\Gacha\\Gacha.png");
            using SKSurface surface = SKSurface.Create(new SKImageInfo(bitmap.Width, bitmap.Height, bitmap.ColorType, bitmap.AlphaType, bitmap.ColorSpace));
            using SKCanvas canvas = surface.Canvas;
            using SKBitmap card2 = SKBitmap.Decode("Resources\\Gacha\\card-2.png").Resize(new SKSize(320, 320).ToSizeI(), SKFilterQuality.High);
            using SKBitmap card3 = SKBitmap.Decode("Resources\\Gacha\\card-3.png").Resize(new SKSize(320, 320).ToSizeI(), SKFilterQuality.High);
            using SKBitmap card4 = SKBitmap.Decode("Resources\\Gacha\\card-4.png").Resize(new SKSize(320, 320).ToSizeI(), SKFilterQuality.High);
            using SKBitmap star = SKBitmap.Decode("Resources\\Gacha\\star.png").Resize(new SKSize(60, 60).ToSizeI(), SKFilterQuality.High);
            canvas.DrawBitmap(bitmap, 0, 0);
            var counterX = 0;
            var counterY = 0;
            foreach (var item in cards)
            {
                using var cardBitmap = SKBitmap.Decode(await client.GetCardThumbPath(item.Item2.ResourceSetName, item.Item1)).Resize(new SKSize(295, 295).ToSizeI(), SKFilterQuality.High);
                canvas.DrawBitmap(cardBitmap, 553.7f + counterX * (320f + 44f) + 12.5f, 390.0f + counterY * (320f + 31f) + 12.5f);
                if (item.Item2.Rarity == 2)
                {
                    canvas.DrawBitmap(card2, 553.7f + counterX * (320f + 44f), 390.0f + counterY * (320f + 31f));
                    canvas.DrawBitmap(star, 553.7f + counterX * (320f + 44f) + 8f, 390.0f + counterY * (320f + 31f) + 208f);
                    canvas.DrawBitmap(star, 553.7f + counterX * (320f + 44f) + 8f, 390.0f + counterY * (320f + 31f) + 256f);
                }
                else if (item.Item2.Rarity == 3)
                {
                    canvas.DrawBitmap(card3, 553.7f + counterX * (320f + 44f), 390.0f + counterY * (320f + 31f));
                    canvas.DrawBitmap(star, 553.7f + counterX * (320f + 44f) + 8f, 390.0f + counterY * (320f + 31f) + 160f);
                    canvas.DrawBitmap(star, 553.7f + counterX * (320f + 44f) + 8f, 390.0f + counterY * (320f + 31f) + 208f);
                    canvas.DrawBitmap(star, 553.7f + counterX * (320f + 44f) + 8f, 390.0f + counterY * (320f + 31f) + 256f);
                }
                else if (item.Item2.Rarity == 4)
                {
                    canvas.DrawBitmap(card4, 553.7f + counterX * (320f + 44f), 390.0f + counterY * (320f + 31f));
                    canvas.DrawBitmap(star, 553.7f + counterX * (320f + 44f) + 8f, 390.0f + counterY * (320f + 31f) + 112f);
                    canvas.DrawBitmap(star, 553.7f + counterX * (320f + 44f) + 8f, 390.0f + counterY * (320f + 31f) + 160f);
                    canvas.DrawBitmap(star, 553.7f + counterX * (320f + 44f) + 8f, 390.0f + counterY * (320f + 31f) + 208f);
                    canvas.DrawBitmap(star, 553.7f + counterX * (320f + 44f) + 8f, 390.0f + counterY * (320f + 31f) + 256f);
                }
                using var typeBitmap = SKBitmap.Decode($"Resources\\Gacha\\{item.Item2.Attribute}.png").Resize(new SKSize(80, 80).ToSizeI(), SKFilterQuality.High);
                canvas.DrawBitmap(typeBitmap, 553.7f + counterX * (320f + 44f) + 236.8f, 390.0f + counterY * (320f + 31f) + 6.4f);
                using var bandBitmap = SKBitmap.Decode($"Resources\\Gacha\\band_{this.data.Characters[item.Item2.CharacterId.ToString()].BandId}.png").Resize(new SKSize(89, 89).ToSizeI(), SKFilterQuality.High);
                canvas.DrawBitmap(bandBitmap, 553.7f + counterX * (320f + 44f), 390.0f + counterY * (320f + 31f) + 3.2f);
                counterX++;
                if (counterX == 5)
                {
                    counterX = 0;
                    counterY++;
                }
            }
            canvas.Save();
            var filePath = $"Resources\\Gacha\\Temp\\{Guid.NewGuid():n}.png";
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using var data = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100);
            await data.AsStream().CopyToAsync(fs);
            return filePath;
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~GachaImageRender()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
