/***************************************************************************
 *
 * AI Training Export Helper
 * 
 * Provides utilities for exporting UO art assets in a format optimized
 * for training Stable Diffusion models.
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Ultima;

namespace UoFiddler.Controls.Helpers
{
    /// <summary>
    /// Type of asset being exported
    /// </summary>
    public enum AIExportAssetType
    {
        StaticItem,
        LandTile,
        Gump,
        Animation
    }

    /// <summary>
    /// Options for AI training export
    /// </summary>
    public class AIExportOptions
    {
        /// <summary>
        /// Target canvas size (512 for SD1.5, 1024 for SDXL)
        /// </summary>
        public int TargetSize { get; set; } = 512;

        /// <summary>
        /// Use white background instead of transparent
        /// </summary>
        public bool UseWhiteBackground { get; set; } = false;

        /// <summary>
        /// Center the sprite on the canvas
        /// </summary>
        public bool CenterSprite { get; set; } = true;

        /// <summary>
        /// Caption template with placeholders: {name}, {index}, {flags}
        /// </summary>
        public string CaptionTemplate { get; set; } = "uo sprite, pixelart, {name}, isometric, rpg game asset";

        /// <summary>
        /// Include tile flags in caption
        /// </summary>
        public bool IncludeFlags { get; set; } = false;

        /// <summary>
        /// Colors to treat as background (will be made transparent)
        /// </summary>
        public List<Color> BackgroundColors { get; set; } = new List<Color>
        {
            Color.FromArgb(255, 0, 0, 0),       // Black
            Color.FromArgb(255, 255, 0, 255)    // Magenta/Pink
        };
    }

    /// <summary>
    /// Helper class for exporting UO assets for AI training
    /// </summary>
    public static class AITrainingExportHelper
    {
        /// <summary>
        /// Removes background colors, replacing them with transparent or white
        /// </summary>
        /// <param name="source">Source bitmap</param>
        /// <param name="options">Export options</param>
        /// <returns>Processed bitmap with background removed</returns>
        public static Bitmap RemoveBackground(Bitmap source, AIExportOptions options)
        {
            if (source == null)
            {
                return null;
            }

            // Create a new 32bpp bitmap for proper alpha support
            Bitmap result = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(result))
            {
                // Fill with target background color first
                g.Clear(options.UseWhiteBackground ? Color.White : Color.Transparent);
            }

            // Copy pixels, making background colors transparent
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color pixel = source.GetPixel(x, y);

                    // Check if this pixel matches any background color
                    bool isBackground = false;
                    foreach (var bgColor in options.BackgroundColors)
                    {
                        if (pixel.R == bgColor.R && pixel.G == bgColor.G && pixel.B == bgColor.B)
                        {
                            isBackground = true;
                            break;
                        }
                    }

                    // Also check for fully transparent pixels
                    if (pixel.A == 0)
                    {
                        isBackground = true;
                    }

                    if (!isBackground)
                    {
                        result.SetPixel(x, y, pixel);
                    }
                    else if (options.UseWhiteBackground)
                    {
                        result.SetPixel(x, y, Color.White);
                    }
                    // If transparent background, pixel is already transparent from Clear()
                }
            }

            return result;
        }

        /// <summary>
        /// Removes background using unsafe pointer access for better performance
        /// </summary>
        public static unsafe Bitmap RemoveBackgroundFast(Bitmap source, AIExportOptions options)
        {
            if (source == null)
            {
                return null;
            }

            // Convert to 32bpp if needed
            Bitmap source32;
            if (source.PixelFormat != PixelFormat.Format32bppArgb)
            {
                source32 = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(source32))
                {
                    g.DrawImage(source, 0, 0, source.Width, source.Height);
                }
            }
            else
            {
                source32 = new Bitmap(source);
            }

            Bitmap result = new Bitmap(source32.Width, source32.Height, PixelFormat.Format32bppArgb);

            // Fill background
            using (Graphics g = Graphics.FromImage(result))
            {
                g.Clear(options.UseWhiteBackground ? Color.White : Color.Transparent);
            }

            Rectangle rect = new Rectangle(0, 0, source32.Width, source32.Height);
            BitmapData srcData = source32.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData dstData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                byte* srcPtr = (byte*)srcData.Scan0;
                byte* dstPtr = (byte*)dstData.Scan0;

                int stride = srcData.Stride;
                int width = source32.Width;
                int height = source32.Height;

                Color bgWhite = Color.White;

                for (int y = 0; y < height; y++)
                {
                    byte* srcRow = srcPtr + (y * stride);
                    byte* dstRow = dstPtr + (y * stride);

                    for (int x = 0; x < width; x++)
                    {
                        byte b = srcRow[0];
                        byte g = srcRow[1];
                        byte r = srcRow[2];
                        byte a = srcRow[3];

                        bool isBackground = false;

                        // Check for transparent
                        if (a == 0)
                        {
                            isBackground = true;
                        }
                        // Check for black background
                        else if (r == 0 && g == 0 && b == 0)
                        {
                            isBackground = true;
                        }
                        // Check for magenta/pink background
                        else if (r == 255 && g == 0 && b == 255)
                        {
                            isBackground = true;
                        }

                        if (!isBackground)
                        {
                            dstRow[0] = b;
                            dstRow[1] = g;
                            dstRow[2] = r;
                            dstRow[3] = a;
                        }
                        else if (options.UseWhiteBackground)
                        {
                            dstRow[0] = 255;
                            dstRow[1] = 255;
                            dstRow[2] = 255;
                            dstRow[3] = 255;
                        }
                        // else: leave as transparent (already cleared)

                        srcRow += 4;
                        dstRow += 4;
                    }
                }
            }
            finally
            {
                source32.UnlockBits(srcData);
                result.UnlockBits(dstData);
                if (source32 != source)
                {
                    source32.Dispose();
                }
            }

            return result;
        }

        /// <summary>
        /// Scales bitmap using Nearest Neighbor interpolation to fit target canvas
        /// </summary>
        /// <param name="source">Source bitmap</param>
        /// <param name="options">Export options</param>
        /// <returns>Scaled and centered bitmap on target canvas</returns>
        public static Bitmap ScaleToCanvas(Bitmap source, AIExportOptions options)
        {
            if (source == null)
            {
                return null;
            }

            int targetSize = options.TargetSize;
            Bitmap result = new Bitmap(targetSize, targetSize, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(result))
            {
                // Set up for crisp pixel art scaling
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.SmoothingMode = SmoothingMode.None;

                // Clear background
                g.Clear(options.UseWhiteBackground ? Color.White : Color.Transparent);

                // Calculate integer scale factor (200%, 300%, 400%, etc.)
                int scaleX = targetSize / source.Width;
                int scaleY = targetSize / source.Height;
                int scale = Math.Min(scaleX, scaleY);
                scale = Math.Max(1, scale); // At least 1x

                int scaledWidth = source.Width * scale;
                int scaledHeight = source.Height * scale;

                int x = 0;
                int y = 0;

                if (options.CenterSprite)
                {
                    x = (targetSize - scaledWidth) / 2;
                    y = (targetSize - scaledHeight) / 2;
                }

                g.DrawImage(source, x, y, scaledWidth, scaledHeight);
            }

            return result;
        }

        /// <summary>
        /// Generates a caption string based on TileData
        /// </summary>
        /// <param name="index">Asset index</param>
        /// <param name="assetType">Type of asset</param>
        /// <param name="options">Export options</param>
        /// <returns>Caption string for training</returns>
        public static string GenerateCaption(int index, AIExportAssetType assetType, AIExportOptions options)
        {
            string name = "";
            string flags = "";

            switch (assetType)
            {
                case AIExportAssetType.StaticItem:
                    if (index < TileData.ItemTable.Length)
                    {
                        var itemData = TileData.ItemTable[index];
                        name = itemData.Name?.Trim() ?? "";
                        if (options.IncludeFlags)
                        {
                            flags = FormatFlags(itemData.Flags);
                        }
                    }
                    break;

                case AIExportAssetType.LandTile:
                    if (index < TileData.LandTable.Length)
                    {
                        var landData = TileData.LandTable[index];
                        name = landData.Name?.Trim() ?? "";
                        if (options.IncludeFlags)
                        {
                            flags = FormatFlags(landData.Flags);
                        }
                    }
                    break;

                case AIExportAssetType.Gump:
                    name = $"gump {index}";
                    break;

                case AIExportAssetType.Animation:
                    name = $"animation {index}";
                    break;
            }

            // Clean up name
            if (string.IsNullOrWhiteSpace(name))
            {
                name = $"unknown {index}";
            }
            name = name.ToLowerInvariant().Replace("_", " ");

            // Apply template
            string caption = options.CaptionTemplate
                .Replace("{name}", name)
                .Replace("{index}", index.ToString())
                .Replace("{flags}", flags);

            return caption.Trim();
        }

        /// <summary>
        /// Formats TileFlags into a readable string for captions
        /// </summary>
        private static string FormatFlags(TileFlag flags)
        {
            var parts = new List<string>();

            if ((flags & TileFlag.Wearable) != 0) parts.Add("wearable");
            if ((flags & TileFlag.Weapon) != 0) parts.Add("weapon");
            if ((flags & TileFlag.Container) != 0) parts.Add("container");
            if ((flags & TileFlag.LightSource) != 0) parts.Add("light source");
            if ((flags & TileFlag.Animation) != 0) parts.Add("animated");
            if ((flags & TileFlag.Wall) != 0) parts.Add("wall");
            if ((flags & TileFlag.Door) != 0) parts.Add("door");
            if ((flags & TileFlag.Roof) != 0) parts.Add("roof");

            return string.Join(", ", parts);
        }

        /// <summary>
        /// Exports a single asset with full processing pipeline
        /// </summary>
        /// <param name="source">Source bitmap</param>
        /// <param name="outputPath">Output file path (without extension)</param>
        /// <param name="index">Asset index</param>
        /// <param name="assetType">Type of asset</param>
        /// <param name="options">Export options</param>
        public static void ExportAsset(Bitmap source, string outputPath, int index,
            AIExportAssetType assetType, AIExportOptions options)
        {
            if (source == null)
            {
                return;
            }

            // Step 1: Remove background
            using (Bitmap bgRemoved = RemoveBackgroundFast(source, options))
            {
                // Step 2: Scale to canvas
                using (Bitmap scaled = ScaleToCanvas(bgRemoved, options))
                {
                    // Step 3: Save PNG
                    string pngPath = outputPath + ".png";
                    scaled.Save(pngPath, ImageFormat.Png);

                    // Step 4: Generate and save caption
                    string caption = GenerateCaption(index, assetType, options);
                    string txtPath = outputPath + ".txt";
                    File.WriteAllText(txtPath, caption);
                }
            }
        }

        /// <summary>
        /// Batch export static items with progress callback
        /// </summary>
        public static int BatchExportStaticItems(int startIndex, int endIndex, string outputFolder,
            AIExportOptions options, Action<int, int> progressCallback = null)
        {
            int exported = 0;

            for (int i = startIndex; i <= endIndex; i++)
            {
                progressCallback?.Invoke(i - startIndex, endIndex - startIndex + 1);

                if (!Art.IsValidStatic(i))
                {
                    continue;
                }

                Bitmap bmp = Art.GetStatic(i);
                if (bmp == null)
                {
                    continue;
                }

                string filename = Path.Combine(outputFolder, $"item_{i:X4}");
                ExportAsset(bmp, filename, i, AIExportAssetType.StaticItem, options);
                exported++;
            }

            return exported;
        }

        /// <summary>
        /// Batch export land tiles with progress callback
        /// </summary>
        public static int BatchExportLandTiles(int startIndex, int endIndex, string outputFolder,
            AIExportOptions options, Action<int, int> progressCallback = null)
        {
            int exported = 0;

            for (int i = startIndex; i <= endIndex; i++)
            {
                progressCallback?.Invoke(i - startIndex, endIndex - startIndex + 1);

                if (!Art.IsValidLand(i))
                {
                    continue;
                }

                Bitmap bmp = Art.GetLand(i);
                if (bmp == null)
                {
                    continue;
                }

                string filename = Path.Combine(outputFolder, $"land_{i:X4}");
                ExportAsset(bmp, filename, i, AIExportAssetType.LandTile, options);
                exported++;
            }

            return exported;
        }

        /// <summary>
        /// Batch export gumps with progress callback
        /// </summary>
        public static int BatchExportGumps(int startIndex, int endIndex, string outputFolder,
            AIExportOptions options, Action<int, int> progressCallback = null)
        {
            int exported = 0;

            for (int i = startIndex; i <= endIndex; i++)
            {
                progressCallback?.Invoke(i - startIndex, endIndex - startIndex + 1);

                if (!Gumps.IsValidIndex(i))
                {
                    continue;
                }

                Bitmap bmp = Gumps.GetGump(i);
                if (bmp == null)
                {
                    continue;
                }

                string filename = Path.Combine(outputFolder, $"gump_{i:X4}");
                ExportAsset(bmp, filename, i, AIExportAssetType.Gump, options);
                exported++;
            }

            return exported;
        }
    }
}
