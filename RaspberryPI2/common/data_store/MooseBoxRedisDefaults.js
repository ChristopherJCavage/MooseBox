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

/**
 * Defines an abstraction for default RedisDB connection constants.
 */
function MooseBoxRedisDefaults() {
    //Public Constants.
    this.DEFAULT_HOSTNAME = redis.default_host;
    this.DEFAULT_PORT = redis.default_port;
}

//Export this class outside this file.
module.exports = MooseBoxRedisDefaults;
