using TerrariaApi.Server;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace BlackFlash;

[ApiVersion (2, 1)]

public class BlackFlash : TerrariaPlugin
    {

    public override string Name => "BlackFlash";
    public override string Author => "Azure5640";
    public override string Description =>
        "% chance of landing a black flash from Jujutsu Kaisen, functioning similar to a critical hit.";
    public override Version Version => new Version(1, 0, 1);

    private Random rand = new Random();
    
    public BlackFlash(Main main) : base(main)
    {
        
    }
    
    public override void Initialize()
    {
        ServerApi.Hooks.NpcStrike.Register(this, blackFlashTrigger);
        GetDataHandlers.PlayerDamage += playerBlackFlash; 
    }

    
    // command stuff
    
    // player black flash

    private void playerBlackFlash(object sender,GetDataHandlers.PlayerDamageEventArgs args)
    {
        TSPlayer attacker = args.Player;

        if (attacker == null) return;
        if (!args.PVP) return;

        TSPlayer playerAttacked = TShock.Players[args.ID];

        if (playerAttacked == null) return;

        int bfChance = rand.Next(0, 100);

        if (bfChance > 50)
        {
            int bfDamage = (int)(Math.Pow(args.Damage, 2.5) - args.Damage);
            
            int newHealth = playerAttacked.TPlayer.statLife - bfDamage;
            playerAttacked.TPlayer.statLife = Math.Max(newHealth, 0);
            

        }
    }
    
    private void blackFlashTrigger(NpcStrikeEventArgs args)
    {
        TSPlayer player = TShock.Players[args.Player.whoAmI];
        if (player == null) return;
        
        int bfChance = rand.Next(0, 100);
        
        if (bfChance > 50)
        {
            // flash

            int bfInformation = Projectile.NewProjectile(Projectile.GetNoneSource(), args.Npc.position.X + 16,
                args.Npc.position.Y + 16, 0f, 0f, ProjectileID.RocketFireworkRed, 0, 0f);
            
            
            args.Damage = (int) Math.Pow(args.Damage,  2.5);
            TSPlayer.All.SendData(PacketTypes.CreateCombatTextExtended, $"BLACK FLASH", unchecked((int)0xFF0000FF), args.Player.position.X, args.Player.position.Y);
            TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", bfInformation );
            TSPlayer.All.SendData(PacketTypes.CreateCombatTextExtended, $"{args.Damage}", (int) (255 << 24 | 255 << 16 | 165 << 8 | 0), args.Npc.position.X, args.Npc.position.Y);
        }
    }

    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.PlayerDamage -= playerBlackFlash;
        }
        
        base.Dispose(disposing);
    }


}