namespace EbonianMod.Core.Registries;

public class Sounds : ModSystem
{
    public static SoundStyle Default = new SoundStyle("EbonianMod/Assets/Sounds/reiFail")
    {
        MaxInstances = 10,
        PitchVariance = 0.3f,
    };


    public const string path = "EbonianMod/Assets/Sounds/";
    public const string outroPath = "EbonianMod/Assets/Music/Outros/";
    public static SoundStyle None;
    public static SoundStyle bloodSpit => Default with
    {
        SoundPath = path + "bloodSpit"
    };

    public static SoundStyle bowPull => Default with
    {
        SoundPath = path + "bowPull",
        Volume = GetInstance<EbonianClientConfig>().BowPullVolume
    };


    public static SoundStyle bowRelease => Default with
    {
        SoundPath = path + "bowRelease"
    };

    public static SoundStyle bigassstar => Default with
    {
        SoundPath = path + "bigassstar"
    };


    public static SoundStyle buzz => Default with
    {
        SoundPath = path + "buzz",
        PitchVariance = 0
    };

    public static SoundStyle cFlameBreath => Default with
    {
        SoundPath = path + "Conglomerate/cFlameBreath",
        MaxInstances = 1,
        IsLooped = true,
        PitchVariance = 0
    };

    public static SoundStyle chargedBeam => Default with
    {
        SoundPath = path + "chargedBeam"
    };


    public static SoundStyle chargedBeamWindUp => Default with
    {
        SoundPath = path + "chargedBeamWindUp",
        PitchVariance = 0f
    };


    public static SoundStyle chargedBeamImpactOnly => Default with
    {
        SoundPath = path + "chargedBeamImpactOnly",
        PitchVariance = 0f
    };


    public static SoundStyle cursedToyCharge => Default with
    {
        SoundPath = path + "cursedToyCharge",
        PitchVariance = 0f
    };


    public static SoundStyle chomp0 => Default with
    {
        SoundPath = path + "chomp0",
        Volume = 1.3f
    };


    public static SoundStyle chomp1 => Default with
    {
        SoundPath = path + "chomp1",
        Volume = 1.3f
    };


    public static SoundStyle chomp2 => Default with
    {
        SoundPath = path + "chomp2",
        MaxInstances = 1
    };


    public static SoundStyle cecitiorOpen => Default with
    {
        SoundPath = path + "Cecitior/cecitiorOpen",
        MaxInstances = 1
    };

    public static SoundStyle cecitiorCloseShort => Default with
    {
        SoundPath = path + "Cecitior/cecitiorCloseShort",
        MaxInstances = 3
    };


    public static SoundStyle cecitiorClose => Default with
    {
        SoundPath = path + "Cecitior/cecitiorClose",
        MaxInstances = 3
    };

    public static SoundStyle cecitiorIdle => Default with
    {
        SoundPath = path + "Cecitior/cecitiorIdle",
        PitchVariance = 0,
        MaxInstances = 1,
        IsLooped = true
    };

    public static SoundStyle cecitiorSlam => Default with
    {
        SoundPath = path + "Cecitior/cecitiorSlam",
        MaxInstances = 3,
        Variants = [0, 1, 2]
    };

    public static SoundStyle cecitiorSlice => Default with
    {
        SoundPath = path + "Cecitior/cecitiorSlice",
        MaxInstances = 3,
        Variants = [0, 1, 2]
    };

    public static SoundStyle cecitiorSpit => Default with
    {
        SoundPath = path + "Cecitior/cecitiorSpit",
        MaxInstances = 3,
        Variants = [0, 1, 2, 3]
    };

    public static SoundStyle crystalArrowForm => Default with
    {
        SoundPath = path + "crystalArrowForm",
        PitchVariance = .25f,
        MaxInstances = 3
    };
    public static SoundStyle deathrayLoop0 => Default with
    {
        SoundPath = path + "deathrayLoop0",
        MaxInstances = 1,
        IsLooped = true,
        PitchVariance = 0
    };
    public static SoundStyle deathrayLoop1 => Default with
    {
        SoundPath = path + "deathrayLoop1",
        MaxInstances = 1,
        IsLooped = true,
        PitchVariance = 0
    };

    public static SoundStyle eggplosion => Default with
    {
        SoundPath = path + "eggplosion"
    };


    public static SoundStyle eruption => Default with
    {
        SoundPath = path + "eruption"
    };
    public static SoundStyle ObeseladBounce => Default with
    {
        SoundPath = path + "ObeseladBounce"
    };
    public static SoundStyle LuminaryConjure => Default with
    {
        SoundPath = path + "LuminaryConjure"
    };
    public static SoundStyle FlyladGroundSlam => Default with
    {
        SoundPath = path + "FlyladGroundSlam"
    };


    public static SoundStyle exolDash => Default with
    {
        SoundPath = path + "exolDash"
    };


    public static SoundStyle exolRoar => Default with
    {
        SoundPath = path + "exolRoar"
    };


    public static SoundStyle exolSummon => Default with
    {
        SoundPath = path + "exolSummon"
    };


    public static SoundStyle flesh0 => Default with
    {
        SoundPath = path + "flesh0"
    };


    public static SoundStyle flesh1 => Default with
    {
        SoundPath = path + "flesh1"
    };


    public static SoundStyle flesh2 => Default with
    {
        SoundPath = path + "flesh2"
    };


    public static SoundStyle buildup => Default with
    {
        SoundPath = path + "buildup",
        PitchVariance = 0
    };


    public static SoundStyle garbageAwaken => Default with
    {
        SoundPath = path + "HotGarbage/garbageAwaken",
        PitchVariance = 0
    };
    
    public static SoundStyle garbageBeep => Default with
    {
        SoundPath = path + "HotGarbage/garbageBeep",
        PitchVariance = 0
    };
    
    public static SoundStyle garbageIgnite => Default with
    {
        SoundPath = path + "HotGarbage/garbageIgnite",
        PitchVariance = 0
    };
    
    public static SoundStyle GarbageThrusterLoop => Default with
    {
        SoundPath = path + "HotGarbage/GarbageThrusterLoop",
        PitchVariance = 0,
        IsLooped = true
    };


    public static SoundStyle garbageSignal => Default with
    {
        SoundPath = path + "HotGarbage/garbageSignal",
        PitchVariance = 0
    };


    public static SoundStyle genericExplosion => Default with
    {
        SoundPath = path + "genericExplosion"
    };


    public static SoundStyle heartbeat => Default with
    {
        SoundPath = path + "heartbeat"
    };

    public static SoundStyle herderDying => Default with
    {
        SoundPath = path + "herderDying",
        PitchVariance = 0,
        IsLooped = true
    };


    public static SoundStyle reiFail => Default with
    {
        SoundPath = path + "reiFail"
    };


    public static SoundStyle reiFail2 => Default with
    {
        SoundPath = path + "reiFail2"
    };


    public static SoundStyle reiTP => Default with
    {
        SoundPath = path + "reiTP"
    };


    public static SoundStyle rolleg => Default with
    {
        SoundPath = path + "rolleg"
    };


    public static SoundStyle rustyAxe => Default with
    {
        SoundPath = path + "rustyAxe"
    };

    public static SoundStyle terrortomaDash => Default with
    {
        SoundPath = path + "Terrortoma/terrortomaDash"
    };


    public static SoundStyle cecitiorDie => Default with
    {
        SoundPath = path + "NPCHit/cecitiorDie"
    };


    public static SoundStyle fleshHit => Default with
    {
        SoundPath = path + "NPCHit/fleshHit"
    };


    public static SoundStyle terrortomaFlesh => Default with
    {
        SoundPath = path + "Terrortoma/terrortomaFlesh"
    };


    public static SoundStyle cecitiorBurp => Default with
    {
        SoundPath = path + "Cecitior/cecitiorBurp"
    };


    public static SoundStyle clawSwipe => Default with
    {
        SoundPath = path + "clawSwipe"
    };


    public static SoundStyle BeamWindUp => Default with
    {
        SoundPath = path + "BeamWindUp",
        PitchVariance = 0
    };


    public static SoundStyle garbageLaser => Default with
    {
        SoundPath = path + "HotGarbage/GarbageLaserLoop",
        PitchVariance = 0,
        IsLooped = true
    };


    public static SoundStyle garbageLaserFire => Default with
    {
        SoundPath = path + "HotGarbage/GarbageLaserFire",
        PitchVariance = 0
    };


    public static SoundStyle garbageNukeArm => Default with
    {
        SoundPath = path + "HotGarbage/GarbageNukeArm",
        PitchVariance = 0
    };


    public static SoundStyle garbageNuke => Default with
    {
        SoundPath = path + "HotGarbage/GarbageNuke",
        PitchVariance = 0
    };


    public static SoundStyle garbageJetBlast => Default with
    {
        SoundPath = path + "HotGarbage/GarbageJetBlast",
        PitchVariance = 0
    };


    public static SoundStyle trumpet => Default with
    {
        SoundPath = path + "trumpet",
        PitchVariance = 0.4f,
        Variants = new int[] { 0, 1 }
    };


    public static SoundStyle terrortomaLaugh => Default with
    {
        SoundPath = path + "Terrortoma/terrortomaLaugh",
        Variants = new int[] { 0, 1 },
        PitchVariance = 0.25f,
        Volume = 1.1f
    };


    public static SoundStyle HeavySwing => Default with
    {
        SoundPath = path + "HeavySwing",
        Variants = new int[] { 0, 1, 2 },
        PitchVariance = 0.25f,
        Volume = 1.1f
    };


    public static SoundStyle FleshImpact => Default with
    {
        SoundPath = path + "FleshImpact",
        Variants = new int[] { 0, 1, 2 },
        PitchVariance = 0.25f,
        Volume = .7f
    };


    public static SoundStyle blink => Default with
    {
        SoundPath = path + "blink"
    };


    public static SoundStyle xSpirit => Default with
    {
        SoundPath = path + "Xareus/xSpirit",
        Volume = 0.7f
    };

    public static SoundStyle xDeath => Default with
    {
        SoundPath = path + "Xareus/xDeath",
        PitchVariance = 0.05f
    };


    public static SoundStyle helicopter => Default with
    {
        SoundPath = path + "helicopter",
        PitchVariance = 0f
    };


    public static SoundStyle firework => Default with
    {
        SoundPath = path + "firework"
    };


    public static SoundStyle magicSlash => Default with
    {
        SoundPath = path + "magicSlash"
    };


    public static SoundStyle sheep => Default with
    {
        SoundPath = path + "sheep",
        PitchVariance = 0.5f,
        Variants = new int[] { 0, 1, 2 }
    };

    public static SoundStyle goat => Default with
    {
        SoundPath = path + "goat",
        PitchVariance = 0.5f,
        Variants = new int[] { 0, 1, 2 }
    };


    public static SoundStyle shears => Default with
    {
        SoundPath = path + "shears",
        PitchVariance = 0.3f,
    };


    public static SoundStyle sheep_player => Default with
    {
        SoundPath = path + "sheep",
        PitchVariance = 0.5f,
        Variants = new int[] { 0, 1 }
    };


    public static SoundStyle shriek => Default with
    {
        SoundPath = path + "shriek"
    };
    
    public static SoundStyle CeaselessRoar => Default with
    {
        SoundPath = path + "Void/CeaselessRoar"
    };
    
    public static SoundStyle BigDarkLightning => Default with
    {
        SoundPath = path + "Void/BigDarkLightning"
    };
    
    public static SoundStyle DarkLightning => Default with
    {
        SoundPath = path + "Void/DarkLightning",
        Variants = [0, 1, 2]
    };
    
    public static SoundStyle CeaselessSuck => Default with
    {
        SoundPath = path + "Void/CeaselessSuck",
        PitchVariance = 0f
    };
    public static SoundStyle CeaselessSuckShort => Default with
    {
        SoundPath = path + "Void/CeaselessSuckShort",
        PitchVariance = 0f
    };
    
    public static SoundStyle CeaselessIdle => Default with
    {
        SoundPath = path + "Void/CeaselessIdle",
        IsLooped = true,
        PitchVariance = 0f
    };


    public static SoundStyle vaccum => Default with
    {
        SoundPath = path + "vaccum",
        IsLooped = true,
        PitchVariance = 0f
    };


    public static SoundStyle ghizasWheel => Default with
    {
        SoundPath = path + "ghizasWheel",
        IsLooped = true,
        PitchVariance = 0f
    };


    public static SoundStyle evilOutro => Default with
    {
        SoundPath = outroPath + "evilOutro",
        PitchVariance = 0,
        MaxInstances = 1,
        Type = SoundType.Music
    };

    public static SoundStyle xareusOutro => Default with
    {
        SoundPath = outroPath + "xareusOutro",
        PitchVariance = 0,
        MaxInstances = 1,
        Type = SoundType.Music
    };
    
    public static SoundStyle voidtitledrop => Default with
    {
        SoundPath = path + "Void/voidtitledrop",
        PitchVariance = 0,
        MaxInstances = 1,
        Type = SoundType.Music
    };
}
