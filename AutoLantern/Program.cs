using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Utils;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace AutoLantern
{
    class Program
    {

        public static Menu menu;
        public static float LastLantern;
        public static Text LanternText;
        public static SpellSlot LanternSlot = (SpellSlot)62;

        static void Main(string[] args)
        {
            Bootstrap.Init(null);
            Loading.OnLoadingComplete += OnLoad;
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is AIHeroClient && sender.IsAlly && args.SData.Name.Equals("LanternWAlly"))
            {
                LastLantern = Environment.TickCount;
                
            }
        }

        public static SpellDataInst LanternSpell
        {
            get { return Player.Spellbook.GetSpell(LanternSlot); }
        }

        private static void OnLoad(EventArgs args)
        {

            if (!ThreshInGame())
            {
                return;
            }


            menu = MainMenu.AddMenu("AutoLantern", "AutoLantern");
            menu.AddLabel(" made by Shimazaki Haruka");
            menu.Add("Auto", new CheckBox("Auto-Lantern at Low HP", true));
            menu.Add("Low", new Slider("Low HP Percent", 20, 10, 50));
            menu.Add("Hotkey", new KeyBind("Hotkey", true, KeyBind.BindTypes.HoldActive, 32));
            //menu.Add("Draw", new CheckBox("Draw Helper Text", true));

            //LanternText = new Text("Click Lantern", "Verdana");

            Game.OnUpdate += OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

        }

        private static void OnGameUpdate(EventArgs args)
        {
            

            if (menu["Auto"].Cast<CheckBox>().CurrentValue && IsLow() && UseLantern() )
            {
                return;
            }

            if (!menu["Hotkey"].Cast<KeyBind>().CurrentValue)
            {
                return;
            }

            UseLantern();
        }

        private static bool UseLantern()
        {
            var lantern =
                ObjectManager.Get<Obj_AI_Base>()
                    .FirstOrDefault(
                        o => o.IsValid && o.IsAlly && o.Name.Equals("ThreshLantern") && Player.Distance(o) <= 500);

            return lantern != null && lantern.IsVisible && Game.TicksPerSecond - LastLantern > 5000 &&
                   Player.Spellbook.CastSpell(LanternSlot,lantern);
        }

        private static bool IsLow()
        {
            return Player.HealthPercent <= menu["low"].Cast<Slider>().CurrentValue;
        }

        private static bool ThreshInGame()
        {
            return HeroManager.Allies.Any(h => !h.IsMe && h.ChampionName.Equals("Thresh"));
        }

    }   
}
