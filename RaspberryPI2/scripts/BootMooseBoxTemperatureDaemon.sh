#!/bin/sh

########################################################################
# Copyright (C) 2015  Christopher James Cavage (cjcavage@gmail.com)    #
#                                                                      #
# This program is free software; you can redistribute it and/or        #
# modify it under the terms of the GNU General Public license          #
# as published by the Free Software Foundation; either version 2       #
# of the License, or (at your option) any later version.               #
#                                                                      #
# This program is distributed in the hope that it will be useful,      #
# but WITHOUT ANY WARRANTY; without even the implied warranty of       #
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        #
# GNU General Public License for more details.                         #
#                                                                      #
# You should have received a copy of the GNU General Public License    #
# along with this program; if not, see <http://www.gnu.org/licenses/>. #
########################################################################

# Setup some important path variables; we might be executed before this is setup.
PATH=/sbin:/bin:/usr/sbin:/usr/bin:/usr/local/sbin:/usr/local/bin

# Start the Temperature Daemon.
forever start /home/pi/MooseBox/daemons/temperature/Temperature.js -d -f /home/pi/MooseBox/daemons/temperature/temperature_daemon.config.js
