
var MooseBoxPubSub = require('./MooseBoxPubSub.js');
var MooseBoxDataStore = require('./MooseBoxDataStore.js');





function onFan0(argument) {
    console.log('Fan 0: ' + argument);
}

function onFan1(argument) {
    console.log('Fan 1: ' + argument);
}

function onFan2(argument) {
    console.log('Fan 2: ' + argument);
}


var mbds = new MooseBoxDataStore('MooseBox', 6379);

var timestamp = Date.now();


mbds.setFanCtrlDaemonVersion('0.0.999', function(err) {
    mbds.getFanCtrlDaemonVersion(function(err, versionStr) {
        console.log('FAN VERSION: ' + versionStr);
    });
});




console.log('THIS TIMESTAMP: ' + timestamp);
console.log('MooseBox Redis Version: ' + mbds.VERSION);

mbds.addTemperatureReading(1001, 42, timestamp + 0, true);
mbds.addTemperatureReading(1001, 43, timestamp + 1, true);
mbds.addTemperatureReading(1001, 44, timestamp + 2, true);
//mbds.addTemperatureReading(1001, 45, timestamp + 3, true);
mbds.addTemperatureReading(1001, 46, timestamp + 4, true);
mbds.addTemperatureReading(1001, 47, timestamp + 5, true);
mbds.addTemperatureReading(1001, 48, timestamp + 6, true);
mbds.addTemperatureReading(1001, 49, timestamp + 7, true, function(err, reply) {

    mbds.queryCurrentTemperature(1001, function(err, serialNumber, reply) {
        console.log('Current Temp: ' + JSON.stringify(reply));
    });

    mbds.queryHistoricalTemperatures(1001, timestamp + 1, timestamp + 6, function(err, serialNumber, reply) {
        console.log('HIST TEMP: ' + JSON.stringify(reply));
    });

    mbds.getFirstLastTemperatureTimestamps(1001, function(err, startTimestamp, endTimestamp) {
        console.log("FIST: " + startTimestamp);
        console.log("LAST: " + endTimestamp);
    });
});


mbds.addFanCtrlReading(0, true, timestamp + 0);
mbds.addFanCtrlReading(0, true, timestamp + 1);
mbds.addFanCtrlReading(0, false, timestamp + 2);
mbds.addFanCtrlReading(0, true, timestamp + 3, function(err, reply) {

    mbds.queryCurrentFanCtrl(0, function(err, fanNumber, reply) {
        console.log('Current Fan: ' + JSON.stringify(reply));
    })

    mbds.queryHistoricalFanCtrl(0, timestamp + 1, timestamp + 2, function(err, fanNumber, reply) {
        console.log('Hist Fan: ' + JSON.stringify(reply));
    });

});

var fobj = {};
fobj.Foo = "bar";

mbds.setFanCtrlConfig(fobj, function(err)
{
    mbds.getFanCtrlConfig(function(err, reply) {
        console.log('Fan CTRL Config: ' + JSON.stringify(reply));
    });

});



var mbps = new MooseBoxPubSub('MooseBox', 6379);

mbps.subscribeFanCtrlReq(0, onFan0);
mbps.subscribeFanCtrlReq(1, onFan1);
mbps.subscribeFanCtrlReq(2, onFan2);

mbps.publishFanCtrlReq(0, true, Date.now());

console.log('Subscribed All');
