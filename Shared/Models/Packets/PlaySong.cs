using System;

namespace PartyPanelShared.Models
{
    [Serializable]
    public class PlaySong
    {
        public string levelId;
        public Characteristic characteristic;
        public Characteristic.BeatmapDifficulty difficulty;
        public PlayerSpecificSettings playerSettings;
        public GameplayModifiers gameplayModifiers;
    }
}
