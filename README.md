# GOG-Games_Automated_Downloading
Simple-to-use command line interface that will automate the sourcing, downloading, and installation of gog.com games. As this is the first version, more made as a (functional) proof-of-concept than anything, the options are limited. 
If anybody somehow stumbles across this little program, finds it useful enough to actually use, and wants features added, feel free to create an issue, I should be able to get to it rather quickly.
If you want a GUI, I'm getting to it, this code is intended to be what runs a Playnite extension. It just turned out to be functional enough as a standalone thing that I felt I could put it here by itself.

Usage: Just double-click the executable, type or paste the gog.com or gog-games.com url when prompted. If you don't see "Done. Press enter to exit", then try running it from powershell or terminal or command prompt, you should see an error message.
Because not all gog games are on gog-games.com, it might error and crash if the game isn't found on gog-games. 

#general info
To get past the captcha on gog-games.com, it uses Selenium, and the way I have it set up, requires Google Chrome to be installed.
The downloads are multithreaded if there are multiple parts
Installs to your user's Saved Games directory, if you want an option to install somewhere else, create an issue.
idk, if theres anything else, really, create an issue.
