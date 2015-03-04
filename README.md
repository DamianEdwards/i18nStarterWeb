TagHelperi18nWeb
===================

ASP.NET 5 Starter Web project template converted to use proposed i18n/l10n system.

### Instructions
To get this prototype working on Visual Studio 2015 CTP6, you'll need to update to later bits of the KRE and packages:

1. Install KVM by following the instructions at https://github.com/aspnet/Home#optimistic
1. Add the ASP.NET vNext NuGet feed by following the instructions at https://github.com/aspnet/Home/wiki/Configuring-the-feed-used-by-kpm-to-restore-packages#updating-feed-on-windows-machines
1. Go to a command line and create a KVM alias called 'dev' for the latest KRE by typing the following command: `kvm upgrade dev`
  2. We're putting this on a separate alias so as to not upset your default VS experience using ASP.NET 5 beta 3 bits
1. Clone this repo locally: `git clone https://github.com/DamianEdwards/i18nStarterWeb.git`
1. Open the i18nStarterWeb.sln file in Visual Studio
1. Ctrl+F5 to run (after projects have loaded, packages haved restored, etc.)
1. Add the following querystring to see the site in French: `?culture=fr`
