using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UoFiddler.Controls.Classes
{
    /// <summary>
    /// Represents the structure of the JSON file for packed animations.
    /// Based on the provided Python scripts.
    /// </summary>
    public class AnimationPackFile
    {
        [JsonPropertyName("meta")]
        public AnimationMetaData Meta { get; set; }

        [JsonPropertyName("frames")]
        public List<AnimationFrameData> Frames { get; set; }
    }

    public class AnimationMetaData
    {
        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("size")]
        public FrameSize Size { get; set; }

        [JsonPropertyName("format")]
        public string Format { get; set; }
    }

    public class FrameSize
    {
        [JsonPropertyName("w")]
        public int W { get; set; }

        [JsonPropertyName("h")]
        public int H { get; set; }
    }

    public class AnimationFrameData
    {
        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("frame")]
        public FrameRect Frame { get; set; }

        [JsonPropertyName("sourceW")]
        public int SourceW { get; set; }

        [JsonPropertyName("sourceH")]
        public int SourceH { get; set; }

        [JsonPropertyName("format")]
        public string Format { get; set; }
    }

    public class FrameRect
    {
        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("w")]
        public int W { get; set; }

        [JsonPropertyName("h")]
        public int H { get; set; }
    }
}
