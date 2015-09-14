# ![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/moose_md.png) MooseBox ![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/raspberry_pi2.png)

Welcome to MooseBox!

MooseBox is a Raspberry PI 2 based platform for regulating cold air using programmable fans and temperature sensors.  The end result is to put pure science behind the process of making homemade soppressata and wine.  It is a joint project between myself (Chris Cavage) and my father (Joseph Cavage).  I am writing all of the software and administrating the Raspberry PI 2 setup while my father is going to do the measurements, wood-working and duct-work for bringing in cold air during the winter months at our Northeast Pennsylvania homestead.

The MooseBox will, or plans to, support all of the following features:
- Independent automation of N programmable fans based on temperature.
- Support for 1...M Temperature Sensors.
- K subscribers to M Temperature Sensors for email notifications for Temperature Alarms based on an end-user's chosen min/max thresholds.
- Accumulation of time-series data for each sensor; aiming to store upwards to 4 months of data for each temperature sensor.
- Provide an iPhone client application with the ability to plot live and historical data, configure temperature alarms, and monitor the power state of fans.

I (https://www.linkedin.com/pub/christopher-cavage/16/bb5/20a) am an extremely passionate, professional Software Engineer who mainly specializes in writing with the C, C++ and C# languages - with enough Python knowledge to be dangerous (e.g. build scripts, etc).  MooseBox allows me to have some real fun on a side project and learn a language I don't use in my day-2-day grind:  JavaScript via node.js. Even though I'm no JavaScript expert, I am trying to write this project "as if" I was writing a professional software project so I'm hoping to have it clean, well commented and easy to understand.  Shoot me an email (cjcavage@gmail.com) if you feel otherwise; I really do try to make everything right!

MooseBox is architected to have separate daemons with separate responsibilities:  N temperature sensors are driven by the temperature daemon, M fans are controlled by the Fan Control daemon, and there is a web service to communicate with the iPhone client using a REST API.  RedisDB is used for both Publish-Subscribe messaging between the processes and for historical time-series data logging (i.e. using the 'Sorted Set' data structure).  Each of the temperature sensors are long-polled at 1.1Hz (which is driven by datasheet specifications).  Loose coupling between the processes from the beginning was done because we might plan to add more sensors to the MooseBox platform, including but not limited to:  humidity sensors and an embedded camera.  I wanted to build something we can grow with from Day #1.

MooseBox is about having fun and curing some delicious charcuteries for eating!!!  Because we love our food and alcohol!!!  A charcuterie such as soppressata might take upwards to 3-4 months to make; without extremely expensive commercial refrigerators it can only really be done during the winter months in northern climates (e.g. Northeast Pennsylvania).  Even then, to make it as delicious as possible, extreme and continuous attention must be paid to keeping the cured meat within a very narrow temperature range.  That's what MooseBox is for!

A MooseBox with two temperature sensors and three fans costs about $240 by the time all the parts are acquired.  Having written a plethora of code for various $5 microcontrollers using I2C sensors before, I realize I could have made better decisions to keep costs lower.  However, doing so would require me to really have an electrical engineer handy and at the time of this writing I was/am a remote employee and do not have such contacts close-by to help!

Besides the Raspberry PI 2, the main hardware components include:
- iButtonLink T-Probe Temperature Sensors: http://www.ibuttonlink.com/pages/temperature-sensors
- iButtonLink USB/RJ-45 Adapters: http://www.ibuttonlink.com/products/linkusb
- YepKit YKUSH programmable USB switches: https://www.yepkit.com/products/ykush
- USB Fans, which you can find of various sizes on amazon.com

As of 2015-Sep-13, I have the following implemented:
- Temperature data acquisition and logging
- Fan control automation and duty cycle tracking
- RedisDB Pub/Sub and Historical time-series data support
- Temperature Alarms

I am currently working on writing the REST API.  Upon completion of the web service, all that remains on the physical MooseBox are some scripts to init everything on launch and some misc Linux administration tasks.  Post that, I will begin on the iPhone client!
