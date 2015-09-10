/************************************************************************
 * Copyright (C) 2015  Christopher James Cavage (cjcavage@gmail.com)    *
 *                                                                      *
 * This program is free software; you can redistribute it and/or        *
 * modify it under the terms of the GNU General Public license          *
 * as published by the Free Software Foundation; either version 2       *
 * of the License, or (at your option) any later version.               *
 *                                                                      *
 * This program is distributed in the hope that it will be useful,      *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        * 
 * GNU General Public License for more details.                         *
 *                                                                      *
 * You should have received a copy of the GNU General Public License    *
 * along with this program; if not, see <http://www.gnu.org/licenses/>. *
 ************************************************************************/
var redis = require('redis');

function DataStore(redisConfigRoot)
{
    //Parameter Validations.
    if (!redisConfigRoot)
        throw "Missing RedisDB config";

    //Create two redis clients: one dedicated to Pub/Sub (see Redis docs); the other for data queries.
    this.m_redisPubSubC = new redis.createClient(redisConfigRoot.Port, redisConfigRoot.Hostname);

    this.m_redisHistDataC = new redis.createClient(redisConfigRoot.Port, redisConfigRoot.Hostname);

    //Subscribe to events appropiately.
    this.m_redisPubSubC.on('connect', this.onRedisConnected.bind(this));
    this.m_redisHistDataC.on('connect', this.onRedisConnected.bind(this));

    this.m_redisPubSubC.on('error', this.onRedisError.bind(this));
    this.m_redisHistDataC.on('error', this.onRedisError.bind(this));

    this.m_redisPubSubC.on('subscribe', function(channel, count)
        {
            //With a subscription ack, now subscribe to all of our channels of interest.
            this.m_redisPubSubC.on('message', this.onRedisMessagePublished.bind(this));
        }.bind(this));

    //PubSub should subscribe to channels now.
    this.m_redisPubSubC.subscribe(redisConfigRoot.PubSub.Sensor0);

    //Set members.
    this.m_redisConfigRoot = redisConfigRoot;

    this.m_tempCelsiusCache = undefined;
}

DataStore.prototype.queryTemperature = function()
{
    //Do not be ambigious with an undefined result.
    if (this.m_tempCelsiusCache)
        return this.m_tempCelsiusCache;
    else
        throw "No data available";
}

DataStore.prototype.onRedisConnected = function()
{

}

DataStore.prototype.onRedisError = function(err)
{

}

DataStore.prototype.onRedisMessagePublished = function(channel, message)
{
    //Every message in our system is JSON; parse it unilaterally.
    var jsonRoot = JSON.parse(message);

    //Now, resolve what channel we are on so we can resolve message.
    switch(channel)
    {
    //Temperature Sensor (0).
    case this.m_redisConfigRoot.PubSub.Sensor0:
        console.log(jsonRoot);

        //Update the current cached value.
        this.m_tempCelsiusCache = jsonRoot.Celsius;

        //TODO: Timeseries.

        break;
    }
}

//Export this class outside this file.
module.exports = DataStore;
