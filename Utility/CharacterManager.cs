namespace DesktopDance.Utility
{
    public static class CharacterManager
    {
        public static List<CharacterEntity> Characters { get; } = new();
        public static float GlobalScale { get; private set; } = 1.0f;
        public static bool GlobalFlip { get; private set; } = false;
        public static bool IsLocked { get; private set; } = false;

        public static void AddCharacter(Bitmap characterBitmap, string name = "Character")
        {
            var character = new CharacterEntity(characterBitmap, Screen.PrimaryScreen!.Bounds, name);
            character.Scale = GlobalScale;
            character.IsFlipped = GlobalFlip;
            Characters.Add(character);
        }

        public static void RemoveCharacter(int index)
        {
            if (index >= 0 && index < Characters.Count)
            {
                Characters[index].Dispose();
                Characters.RemoveAt(index);
            }
        }

        public static void ClearCharacters()
        {
            for (int i = Characters.Count - 1; i >= 0; i--)
            {
                Characters[i].Dispose();
                Characters.RemoveAt(i);
            }
        }

        public static void SetGlobalScale(float scale)
        {
            GlobalScale = scale;
            foreach (var character in Characters)
            {
                character.Scale = scale;
            }
        }

        public static void SetGlobalFlip(bool flip)
        {
            GlobalFlip = flip;
            foreach (var character in Characters)
            {
                character.IsFlipped = flip;
            }
        }

        public static void SetLocked(bool locked)
        {
            IsLocked = locked;
        }
    }
}
