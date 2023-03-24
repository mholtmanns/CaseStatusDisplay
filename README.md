# CaseStatusDisplay
Tools and Apps to enable a Raspberry Pi driven small LCD Display to show various PC stats and more.

## Problem statement
I am in the process of building a new PC rig and wanted to look for new challenges. Watercooling is a given since it is so much fun (if you have the budget), RGB lighting is nearly unavoidable nowadays as well, so, yeah.

I also had a very old pre-modding-times Antec case I wanted to refurbish to fit the setup I had in mind. That in itself has some challenges, but that is a different story.

So the one thing I hadn't done in the past was try and integrate a status display somewhere in or on the case. I decided to go for a small 5 inch LCD, but to have more options for my sensors, drive it via a Raspberry Pi Zero W I still have lying around.

Here are some examples:

[RasPi solution using HWInfo data](./assets/CaseDisplay1.jpg) -
https://www.tomshardware.com/news/raspberry-pi-system-monitor-pc-hardware

[Internal but connected to the PC](./assets/CaseDisplay2.jpg) -
https://www.downloadsource.net/how-to-create-your-own-custom-desktop-computer-stat-screen/n/10288/

[Another Internal/PC output solution](./assets/CaseDisplay3.jpg) -
https://www.youtube.com/watch?v=RcNJA6V4tNY

[External solution](./assets/CaseDisplay4.jpg) -
https://dribbble.com/shots/18602418-PC-Status-Monitor

That means I need some software to
- read sensor data from the PC or sensors directly connected to the RasPi
- send it to the RasPi
- display it nicely on the RasPi screen

There are commercial solutions available like
[AIDA64](https://www.aida64.com/products/features/external-display-support)
or Plug-In based setups using [HWInfo](https://www.hwinfo.com/), but that as well is non-free if you want to go beyond the 12 hour shared memory restriciton in the free version. On top of that everything I found that would provide what I would like to see was tied to either of these two software solutions. Given the monthly subscription that would require it was a no-go.

## Approach to implement
First of all, I checked out if there where similar Open Source or enthusiast projects out there which might fit my needs. The only one I came across that included RasPi support was [MoBro](https://www.mod-bros.com/en/projects/mobro). It supported free monitoring software and came with a nice display scheme and it's own RasPi OS image. And better support and features if you supported teh team through Patreon. *sigh* Not that I am unwilling to pay for good quality services or products, but monthly subscriptions are getting out of hand lately.

Another downside was that I could not get the MoBro OS image to work with my specific LCD in portrait mode. I could probably figure it out, but what finally shut down this solution was how to get the RasPi MoBro display to show *local* data from the Pi itself.

So I came up with a design that suited my needs **AND** stays on the Open Source side all the way through.

The planned data flow looks roughly like this:
- Have a monitoring software running on the PC that gathers data from the PC parts (GPU, CPU, Fans, ...) of interest to me
- Send that data to a Websocket server somewhere on the network (The websocket server can also run on the RasPi) *Security concerns? Why? It's non-critical data only and in a private network*
- Have the Raspberry Pi run a websocket client which reads the broadcast messages and translates them in status updates
- In the same fashion gather data from whatever sensors are connected to the RasPi and send it to the server (or digest them in the same app the server is running in)
- Use a simple JavaScript/SVG fullscreen web page on the RasPi to display the data

### Monitoring software
MoBro works with [OpenHarwareMonitor](https://github.com/openhardwaremonitor/openhardwaremonitor), which does not seem to be well maintained anymore. But it has a fork called [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor), which also includes a Library version for use in third-party applications.

### Websockets
The simplest way to do this and utilize simple access to the RasPi GPIOs is a python script that runs on the Pi and opens a websocket server. While at the same time (or as a seperate script) polling GPIO pins for sensor data.

### Data display
This could be done as a "down-to-metal" graphical application usign for example [RayLib](https://www.raylib.com/) as schown in [this Blog from 2019](https://avikdas.com/2019/01/23/writing-gui-applications-on-raspberry-pi-without-x.html). But since I wanted to get this done still in this century, I decided to use a Web based approach instead.

So I chose javaScript with SVG and went out looking. One of the nicer solutions for Gauge like representations came from [CodeProject and is called GaugeSVG](https://www.codeproject.com/Articles/604502/A-Universal-Gauge-for-Your-Web-Dashboard). It is highly customizable and easy to animate. And it's free (under [CPOL](https://www.codeproject.com/info/cpol10.aspx)).

However, this only gives me Gauges whereas I would like to have other animations as well, but it is a good starting point.

The browser of choice is [Midori](https://en.wikipedia.org/wiki/Midori_(web_browser)) since with the latest update of chromium-browser on RaspiOS to version 109.0.5414.112 they seemed to have dropped ArmV6 support and it does nto run on the Zero W anymore. I could try the earlier version 104.0.5112, but Midori works ok so far.

## Directory structure
- [assets](./assets/) Contains informational files like images or PDF files that help with the design and implementations process
- [docs](./docs) Docs will contain both Hardware and Software documentation
- [JSDisplay](./JSDisplay/) Contains the HTML and JavaScript files that will display the incoming data on the LCD
- [MonitorServer](./MonitorServer/) Websocket server in Python that receives and broadcasts all monitoring data
- [WinMon](./WinMon/) The C# wrapper around LibreHardwareMonitor to gather PC sensor data and send it to the websocket server

# Status
## 24.03.2023
- Websocket communication is working locally
  - RasPi websocket connection was not yet tested
- RasPi is set up in Portrait mode and displays the GaugeSVG samples
- [./JSDisplay/monitoring_graph.html](./JSDisplay/monitoring_graph.html) displays a prototype of a running bandwidth monitor
