# Akane Discord Bot

<p align="center">
  <br>
  <a href="https://github.com/XSaitoKungX"><img src="/Images/Akane.jpg" height="500" alt="Akane Discord Bot"></a>
  <br>
  Ein multifunktionaler Discord-Bot, entwickelt mit ❤ von [XSaitoKungX](https://github.com/XSaitoKungX)
  <br>
</p>

## Willkommen 👋

Willkommen beim Akane Discord Bot! Dieser vielseitige Bot bietet deinem Discord-Server echten Mehrwert. Akane unterstützt sowohl Prefix- als auch Slash-Commands, um sicherzustellen, dass du und deine Community auf die Art und Weise interagieren können, die am besten zu euch passt.

## Ressourcen Links 📚

- [Link zur Dokumentation](https://dsharpplus.github.io/DSharpPlus/)

## Beschreibung

Der Akane Discord Bot ist ein leistungsstarker, benutzerfreundlicher Bot, der entwickelt wurde, um deinem Server Funktionalitäten hinzuzufügen und das Discord-Erlebnis deiner Community zu verbessern. Mit einer breiten Palette von Funktionen, darunter Slash-Commands und einfache Musikbefehle, ist Akane die perfekte Ergänzung für jeden Discord-Server.

## Funktionen 🚀

- **Slash Commands**: Nutze die neuesten Discord-Features mit Slash-Commands, um Befehle auf eine intuitive Weise auszuführen.
- **Einfache Aktualisierung**: Halte deinen Bot immer auf dem neuesten Stand, um von den neuesten Funktionen und Verbesserungen zu profitieren.
- **Musikbefehle**: Genieße Musik in deinem Voice-Channel mit einfachen Musikbefehlen.
- **Benutzerfreundlich**: Akane ist einfach zu verwenden und bietet eine reibungslose Erfahrung für alle Benutzer.

## To-Do 📝

- [ ] Automoderation
- [ ] Benutzerdefinierte Befehle
- [ ] Erweiterte Musikbefehle
- [ ] Ticketsystem
- [ ] Nützliche Befehle
- [ ] Vorschläge
- [ ] Reaktionsrollen
- [ ] Familienfunktionen
- [ ] Gewinnspiele
- [ ] Möchtest du den Bot nicht selbst hosten?

## Anforderungen 🌐

Bevor du Akane verwenden kannst, stelle sicher, dass du folgende Anforderungen erfüllst:

- [Visual Studio](https://visualstudio.microsoft.com/de/thank-you-downloading-visual-studio/?sku=Community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030&workload=dotnet-dotnetwebcloud&passive=false#dotnet)
- [.NET Core 3.1](https://dotnet.microsoft.com/en-us/download/dotnet/3.1)
- [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus)
- Discord-Token: Erhalte ihn von der [Discord Developer-Plattform](https://discord.com/developers/applications)
- Giphy-API-Token: Erhalte ihn von der [Giphy Developer-Plattform](https://developers.giphy.com/)
- API-Schlüssel für OpenAI (für den Chatbot): Erhalte ihn von der [OpenAI Developer-Plattform](https://beta.openai.com/account/api-keys)

## Locales 🌎

Derzeit verfügbare Gebietsschemata sind:

- German (de)

## Installationsanleitung 🏁

Folge diesen Schritten, um Akane auf deinem Server zu installieren:

1. [Fork das Repository](https://github.com/XSaitoKungX/AkaneBot/fork)
2. Clone dein Fork: `git clone https://github.com/dein-benutzername/AkaneBot.git`
3. Erstelle einen Feature-Branch: `git checkout -b mein-neues-feature`
4. Stage deine Änderungen: `git add .`
5. Commit deine Änderungen: Verwende `cz` oder `npm run commit`, aber nicht `git commit`
6. Push deinen Branch: `git push origin mein-neues-feature`
7. Erstelle eine Pull-Anfrage

## Get Started 🏁

Verwende den folgenden Code, um Akane in deinem Projekt zu integrieren:

```csharp
using DSharpPlus;
using DSharpPlus.CommandsNext;
using System;

class Program
{
    static async Task Main(string[] args)
    {
        var discord = new DiscordClient(new DiscordConfiguration
        {
            // Füge deine Konfiguration hier ein
        });

        var commands = discord.UseCommandsNext(new CommandsNextConfiguration
        {
            // Füge deine Befehlskonfiguration hier ein
        });

        commands.RegisterCommands<DeinBefehlsmodul>();

        await discord.ConnectAsync();
        await Task.Delay(-1);
    }
}
```

## Help & Support 🤝

Wenn du Unterstützung bei der Einrichtung oder Verwendung von Akane benötigst, stehe ich dir gerne zur Verfügung. Du kannst mich im Kommentarbereich erreichen oder in meinem Discord-Server, wo du weitere Hilfe und Unterstützung erhalten kannst.

- [Trete meinem Discord-Server bei](https://discord.gg/NDfK6NPZVZ)
- [Lade den Bot ein](https://discord.com/api/oauth2/authorize?client_id=1155480674707460220&permissions=8&scope=applications.commands%20bot)

## Mitwirkende 🤝

- [XSaitoKungX](https://github.com/XSaitoKungX)

Natürlich, ich kann dir helfen, den "Get Started" Abschnitt zu erweitern und die Lizenzinformationen zu ergänzen. Hier ist der aktualisierte Abschnitt:

## Lizenz 📝

Dieses Projekt ist unter der [MIT-Lizenz](LICENSE) lizenziert.

Die MIT-Lizenz ist eine Open-Source-Lizenz, die dir erlaubt, das Projekt in deinen eigenen Projekten zu verwenden, zu ändern und zu verteilen, solange du die Lizenzbedingungen einhältst. Weitere Informationen findest du in der [Lizenzdatei](LICENSE).

```plaintext
MIT License

Urheberrecht (c) [2023] [XSaitoKungX]

Die Erlaubnis wird hiermit kostenlos erteilt, jede Person, die eine Kopie dieser Software und der zugehörigen Dokumentationsdateien (die "Software") erhält, sie ohne Einschränkung zu verwenden, zu kopieren, zu ändern, zusammenzuführen, zu veröffentlichen, zu verteilen, zu sublizenzieren und/oder zu verkaufen, und Personen, denen die Software zur Verfügung gestellt wird, dies unter den folgenden Bedingungen zu gestatten:

Der obige Urheberrechtshinweis und dieser Erlaubnishinweis müssen in allen Kopien oder wesentlichen Teilen der Software enthalten sein.

DIE SOFTWARE WIRD OHNE MÄNGELGEWÄHR UND OHNE JEGLICHE AUSDRÜCKLICHE ODER STILLSCHWEIGENDE GEWÄHRLEISTUNG, EINSCHLIESSLICH, ABER NICHT BESCHRÄNKT AUF DIE GEWÄHRLEISTUNG DER MARKTFÄHIGKEIT, DER EIGNUNG FÜR EINEN BESTIMMTEN ZWECK UND DER NICHTVERLETZUNG VON RECHTEN DRITTER, BEREITGESTELLT. IN KEINEM FALL HAFTEN DIE AUTOREN ODER COPYRIGHT-INHABER FÜR JEGLICHEN SCHADEN ODER SONSTIGE HAFTUNG, OB AUS DEM VERTRAG, DELIKT ODER ANDEREN VERHALTENSWEISEN, DIE SICH AUS, IN VERBINDUNG MIT DER SOFTWARE ODER DER NUTZUNG ODER SONSTIGEN VERWENDUNG DER SOFTWARE ERGEBEN.
```
