# MooseBox
Welcome to MooseBox!

MooseBox is a fun little side project that I am doing along with my father (Joseph Cavage).  The end goal is to be able to automate temperatures for the purposes of curing homemade soppressata, which is a Northeast Pennsylvania regional favorite type of charcuterie.  Our family also makes homemade wines, and so the MooseBox will be used for automating that as well.  I (Chris) am building out all the computing hardware sensors and writing all of the software for the project while my father (Joseph) is going to build out the mechanical apparatus.

MooseBox is built on top of a Raspberry PI 2 running the default Raspbian distro.  It contains a collection of daemons which integrate with the different sensors (e.g. temperature, USB fans, etc) and a master web-service.  The temperature daemon monitors 1...N iButtonLink LinkUSB devices each containing 1...N iButtonLink DS18B20 Temperature sensors (i.e. T-Sensor/T-Probe).  Meanwhile, the fan control daemone controls 1...3 USB fans using a YepKit YKUSH programmable USB switch.  Locally, all the daemons and web-service are hooked up via a Pub/Sub backed by RedisDB.

Since neither curing soppressata nor letting wine rest requires more than 3-4 months, we are currently recording every temperature reading and using the RedisDB 'SortedSet' structure for time-series data.  Temperature readings come in at about 1.1Hz.  Other statistics are also being logged in RedisDB for accurate food automation.

the second portion of the MooseBox project is an iPhone application (TBD) which allow our family to monitor the current temperatures, state of cooling fans, and plot historical data to look for trends.
