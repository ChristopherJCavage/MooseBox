var TemperatureAlarm = require('./TemperatureAlarm.js');
var FanAutomation = require('./FanAutomation.js');

var fte = new FanAutomation();

fte.register(1234, 1, 50);
fte.register(1234, 2, 60);
fte.register(1234, 3, 70);

fte.register(4321, 4, 50);
fte.register(4321, 5, 50);
fte.register(4321, 6, 50);

fte.register(9999, 7, 50);

var i = 1;
console.log(i++ + ': ' + JSON.stringify(fte.getPowerStateInstructions(1234, 49)));
console.log(i++ + ': ' + JSON.stringify(fte.getPowerStateInstructions(1234, 50)));
console.log(i++ + ': ' + JSON.stringify(fte.getPowerStateInstructions(1234, 51)));
console.log(i++ + ': ' + JSON.stringify(fte.getPowerStateInstructions(1234, 80)));
console.log(i++ + ': ' + JSON.stringify(fte.getPowerStateInstructions(1234, 70)));
console.log(i++ + ': ' + JSON.stringify(fte.getPowerStateInstructions(4321, 49)));
console.log(i++ + ': ' + JSON.stringify(fte.getPowerStateInstructions(4321, 50)));
console.log(i++ + ': ' + JSON.stringify(fte.getPowerStateInstructions(4321, 51)));
console.log(i++ + ': ' + JSON.stringify(fte.getPowerStateInstructions(9999, 51)));
console.log(i++ + ': ' + JSON.stringify(fte.getPowerStateInstructions(0, 9999)));

fte.unregisterSensor(4321);
fte.unregisterFan(2);

console.log('A: ' + JSON.stringify(fte.getPowerStateInstructions(4321, 51)));

console.log('Fans: ' + JSON.stringify(fte.getRegisteredFans()));
console.log('S#s: ' + JSON.stringify(fte.getRegisteredTemperatureSensors()));
console.log(i++ + ': ' + JSON.stringify(fte.getPowerStateInstructions(1234, 80)));


console.log('CONFIG: ' + JSON.stringify(fte.getConfigObj()));

var fte2 = new FanAutomation(fte.getConfigObj());

console.log('++++++++++++++++++++');
console.log('CONFIG: ' + JSON.stringify(fte.getConfigObj()));
/*
var ta = new TemperatureAlarm(0, null);

ta.register(1, 10, 20, '1@10-20');
ta.register(2, 15, 25, '2A@15-25');
ta.register(2, 15, 25, '2B@15-25');
ta.register(2, 5, 50, '3@5-50');
ta.register(3, 5, 50, '1@10-20');

console.log('R1: ' + JSON.stringify(ta.getRegisteredEmailAddresses()));

console.log('R4: ' + JSON.stringify(ta.getRegisteredEmailAddresses(3)));

ta.unregisterEmailAddress('2B@15-25');
ta.unregisterEmailAddress('1@10-20', 3);

console.log('R2: ' + JSON.stringify(ta.getRegisteredEmailAddresses(2)));
console.log('R3: ' + JSON.stringify(ta.getRegisteredTemperatureSensors()));
console.log('R5: ' + JSON.stringify(ta.getRegisteredEmailAddresses(3)));
console.log('R5: ' + JSON.stringify(ta.getRegisteredEmailAddresses(1)));

ta.unregisterSensor(2);

console.log('R6: ' + JSON.stringify(ta.getRegisteredTemperatureSensors()));

var eas = ta.checkReading(1, 9); 
for(i = 0; i < eas.length; i++) console.log('> ' + eas[i]);

console.log('\n');
eas = ta.checkReading(1, 10); 
for(i = 0; i < eas.length; i++) console.log('> ' + eas[i]);


console.log('\n');
eas = ta.checkReading(2, 26); 
for(i = 0; i < eas.length; i++) console.log('> ' + eas[i]);

console.log('\n');
eas = ta.checkReading(2, 51); 
for(i = 0; i < eas.length; i++) console.log('> ' + eas[i]);

ta.unregister(2, '2B@15-25');
console.log('unregister');

console.log('\n');
eas = ta.checkReading(2, 51); 
for(i = 0; i < eas.length; i++) console.log('> ' + eas[i]);

var ff = ta.getAlarmsConfig();

console.log('ff: ' + JSON.stringify(ff, null, 2));

var ta2 = new TemperatureAlarm(0, ff);

console.log('ta: ' + JSON.stringify(ta, null, 2));

console.log('ta2: ' + JSON.stringify(ta2, null, 2));
*/