using System;

namespace Core
{
    /// <summary>
    /// Represents the various user roles in the project.
    /// </summary>
    [Flags]
    public enum UserRole
    {
        None = 0,
        Programmer = 1 << 1,
        GameplayProgrammer = 1 << 2,
        ToolDeveloper = 1 << 3,
        GameDesigner = 1 << 4,
        LevelDesigner = 1 << 5,
        FxArtist = 1 << 6,
        SoundDesigner = 1 << 7,
        Animator = 1 << 8,
        Artist = 1 << 9,
        QualityAssurance = 1 << 10,
        CharacterArtist = 1 << 11,
        LightingArtist = 1 << 12,
        Cinematographer = 1 << 13,

        // Combinations //
        GraphicsRoles = Artist | CharacterArtist | Cinematographer | LightingArtist | FxArtist | Programmer,
        GameplayRoles = Animator | GameDesigner | GameplayProgrammer | LevelDesigner | SoundDesigner | Programmer,

        All = int.MaxValue
    }
    
}
