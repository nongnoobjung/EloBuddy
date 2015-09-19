using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Support_Mode
{
    class Program
    {
        private static Menu menu;
        private static AIHeroClient Player;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            menu = MainMenu.AddMenu("SupportMode", "supportmode");
            menu.AddGroupLabel("SupportMode");
            menu.AddSeparator();
            menu.AddLabel("by Shimazaki Haruka");
            menu.AddSeparator();
            menu.Add("enable", new CheckBox("Enable", true));
            menu.Add("range", new Slider("Distance from allies",1400, 700, 2000));

            Chat.Print("Support Mode by ShimazakiHaruka");

            Orbwalker.OnPreAttack += BeforeAttack;

        }

        private static void BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (enable)
            {
                var lasthitmode = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);

                if (lasthitmode)
                {
                    return;
                }


                if(target.Type == GameObjectType.obj_AI_Minion)
                {
                    var allyinrange = HeroManager.Allies.Count(x => !x.IsMe && x.Distance(Player) <= range);
                    if(allyinrange > 0)
                    {
                        args.Process = false;
                    }
                }
            }

        }

        private static bool enable
        {
            get { return menu.Get<CheckBox>("enable").CurrentValue; }
        }

        private static int range
        {
            get { return menu.Get<Slider>("range").CurrentValue; }
        }
    }
}
