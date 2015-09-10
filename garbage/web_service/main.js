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
var argv = require('minimist')(process.argv.slice(2));
var DataStore = require('./DataStore.js');
var fs = require('fs');

function main(argv)
{
        //Everything from hereon requires configuration file.
        var configFile = "./webservice.config.json";

        if (argv.f !== undefined)
            configFile = argv.f;

        var jsonText = fs.readFileSync(configFile, 'utf8');

        var jsonRoot = JSON.parse(jsonText);

    var dataStore = new DataStore(jsonRoot.Redis);


}


/*
//Setup the main app using express and setup parsing.
var app = express();
var port = process.env.PORT || 8080;
var router = express.Router();



app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());

//Prefex all routes with /MooseBox and register all routes.
app.use('/MooseBox', router);

//Start the server.
console.log('Starting MooseBox Web-Service.')

app.listen(port);
*/

main(argv);
