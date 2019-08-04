using System;

namespace PartyPanelShared.Models
{
    [Serializable]
    public class SongList
    {
        public PreviewBeatmapLevel[] Levels { get; set; }
    }
}
