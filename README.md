# ![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/moose_md.png) MooseBox ![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/raspberry_pi2.png)

Welcome to MooseBox!

MooseBox is a Raspberry PI 2 based platform for regulating cold air using programmable USB fans and temperature sensors. The end result is to put pure science behind the process of making homemade soppressata (Northeast Pennsylvania style) and wine. It is a joint project between myself (Chris Cavage) and my father (Joseph Cavage). I am writing all of the software and administrating the Raspberry PI 2 setup while my father is going to do the measurements, wood-working and duct-work for bringing in cold air during the winter months at our Northeast Pennsylvania homestead.

Because we make wine in the fall months and soppressata in the winter months in this region, MooseBox is really our hobby project that we work on once a year, with the first functional release at Thanksgiving of 2015.  We intend to add to it a little bit every year (see bottom for list of planned features).

The MooseBox Server (i.e. Raspberry Pi 2) supports the following:
- Support for 1...M Temperature Sensors.
- Independent automation of N programmable USB Fans based on temperature thresholds with a 1-to-Many relationship of Fans to Temperature Sensors.
  - We currently have the MooseBox regulating temperatures in both the wine box and the soppressata box.
- Manual user overrides of N programmable USB Fans.
- K subscribers to M Temperature Sensors for email notifications for Temperature Alarms based on an end-user's chosen min/max thresholds.
- Accumulation of time-series data for each sensor; aiming to store upwards to 4 months of data for each temperature sensor using RedisDB.

The MooseBox Client (i.e. Dell economy laptop) supports the following:
- System status panel to easily monitor all power states of fans, temperature and memory status.
- Configuration of Fan automation using easily identifiable temperature sensors.
- Registration/Unregistration of a user's email address for MooseBox temperature alarms.
- Manual Fan override panel.
- Historical temperature data plots configurable via an end-user's selection of time ranges.

For the MooseBox client, the original plan was to have the first release written with a Xamarin C# iPhone client but during the time of writing my wife and I were preparing for a cross-country move from Philadelphia to Seattle and so I chose instead to just leverage their economy laptop with a Windows client which could be written (much) faster due to our time constratints.  Also, we had to spend many hours on the final construction and testing of the actual boxes, ducts, and ensuring everything was properly sealed.  The next highest software priority of MooseBox is an iPhone client.

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/MooseBoxSystemOverview.png)
___
![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/MooseBoxHighLevelSoftwareArchitecture.png)

I am an extremely passionate, professional Software Engineer who mainly specializes in writing with the C, C++ and C# languages - with enough Python knowledge to be dangerous (e.g. build scripts, etc). MooseBox allows me to have some real fun on a side project and learn a language I don't use in my day-2-day grind: JavaScript via node.js. Even though I'm no JavaScript expert, I am trying to write this project "as if" I was writing a professional software project so I'm hoping to have it clean, well commented and easy to understand. Shoot me an email (cjcavage@gmail.com) if you feel otherwise; I really do try to make everything right!  I love playing around on embedded Linux devices like Raspberry PIs and Beagle Bone Blacks!

MooseBox is architected to have separate daemons with separate responsibilities: N temperature sensors are driven by the temperature daemon, M fans are controlled by the Fan Control daemon, and there is a web service to communicate with the outside world using a REST API. RedisDB is used for both Publish-Subscribe messaging between the processes and for historical time-series data logging (i.e. using the 'Sorted Set' data structure). Each of the temperature sensors are long-polled at 1.1Hz (which is driven by datasheet specifications). Loose coupling between the processes from the beginning was done because we might plan to add more sensors to the MooseBox platform, including but not limited to: humidity sensors and an embedded camera. I wanted to build something we can grow with from Day #1.

MooseBox is about having fun and curing some delicious charcuteries for eating and making wine!!! Because we love our food and alcohol!!! A charcuterie such as soppressata might take multiple months to make; without extremely expensive commercial refrigerators it can only really be done during the winter months in northern climates (e.g. Northeast Pennsylvania). Even then, to make it as delicious as possible, extreme and continuous attention must be paid to keeping the cured meat within a very narrow temperature range; this precision is necessary because the meat is fermented.  The wine requires precise temperature controls because our homemade wine is made as organic as possible without the use of sulfites, which will spoil if the wine reaches room temperature during the fermenting process.

A MooseBox with two temperature sensors and three fans costs about $240 by the time all the parts, wires, SD cards and WIFI dongle is acquired. Having written a plethora of code for various $5 microcontrollers using I2C sensors before, I realize I could have made better decisions to keep costs lower. However, doing so would require me to really have an electrical engineer handy and at the time of this writing I was/am a remote employee and do not have such contacts close-by to help!

Besides the Raspberry PI 2, the main hardware components include:
- iButtonLink Temperature Sensors: http://www.ibuttonlink.com/pages/temperature-sensors
- iButtonLink USB/RJ-45 Adapters: http://www.ibuttonlink.com/products/linkusb
- YepKit YKUSH programmable USB switches: https://www.yepkit.com/products/ykush
- USB WiFi Dongle recommended by Adafruit: http://www.adafruit.com/products/814
- USB Fans, which can be found at https://www.amazon.com

Dec 2015, First Release of MooseBox:
- Temperature data acquisition and logging
- Fan control automation and duty cycle tracking
- RedisDB Pub/Sub and Historical time-series data support
- Temperature Alarms
- REST API
- Windows Client GUI on leftover Dell economy laptop
- Separated, insulated, wood-framed, large “boxes” for soppresata and wine making
- Duct work to bring in cold winter air from outside with specialized molds for USB Fans placed in series

Future Releases of MooseBox for 2016, 2017 tinkering:
- Xamarin C# iPhone Client (all non-UI C# client code written with portability in mind)
- Humidity Sensors
- Embedded Camera
- Larger, and more boxes
- More duct-working
- Experimenting with new sausage recipes!
- Homemade beer brewing???

Following are some pictures to showing the MooseBox during different phases:

**20150901:** Early development w/splayed out hardware and a couple fans and a temperature sensor.
![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/MooseBox_HardwareSplayOut_A.png)

**20150915:** Devs-Eye-View of the whole Pub/Sub working with Fan Automation:
![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/PubSub_DevelView.png)

**20150930:** Early progress of the wood framework for sausage curing racks:
![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/201509_MooseBox.png)

**20151126:** Windows client screenshots on Dell economy laptop for temperature monitoring, fan automation, temperature alarms, fan power on/off manual overrides, and plotting of historical temperature data.
![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/MainCtrlPanel_RevA.PNG)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/FanAutomationPanel_RevA.png)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/ViewRegisteredAlarms_RevA.png)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/CreateNewTemperatureAlarm.PNG)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/FanManualOverride_RevA.PNG)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/PlotTemperatureData_RevA.png)

**20151126:** Example of MooseBox Temperature Alarm Email
![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/MooseBoxTempAlarmEmail_RevA.PNG)

**20151127:** Final MooseBox imagery during construction completion
![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/MooseBoxControlModule_RevA.png)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/DuctFan2_RevA.png)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/DuctFan_RevA.png)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/TopMooseBox_RevA.png)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/MooseBoxInner_RevA.png)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/MooseBoxBoxes_2_RevA.png)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/MooseBoxBoxes_1_RevA.png)

**20151128:** My Wife (Sabrina) and Mother (Barbara) painting the MooseBox to make it "Pretty"

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/MooseBox_Painting_3.png)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/MooseBox_Painting_2.png)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/MooseBox_Painting_1.png)

**20151129:** Pictures of the final painted MooseBox and homemade wines.

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/PaintedMooseBox_1.png)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/PaintedMooseBox_2.png)

![alt tag](https://github.com/ChristopherJCavage/MooseBox/blob/master/documentation/assets/WineInMooseBox.png)
