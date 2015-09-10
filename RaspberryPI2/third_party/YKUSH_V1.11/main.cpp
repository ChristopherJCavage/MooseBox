/****************************************************************************
 FileName:      main.cpp
 Dependencies:  See INCLUDES section
 Compiler:      g++
 Company:       Yepkit, Lda.

 Software License Agreement:

 Copyright (c) 2014 Yepkit Lda

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.

*****************************************************************************
 File Description:

    Change History:
        Rev     Date            Description
        ----    -----------     -----------------------------------------
        1.0     2014-08-10      Firs Release


 ****************************************************************************
 *  Summary:
 *      Main program function
 *
 *
 *
*****************************************************************************/



#include <cstdlib>
#include <iostream>
#include <stdio.h>

#include "usbcom.h"

using namespace std;

/*
 * 
 */
int main(int argc, char** argv) {

    char choice;

    
    char cmd = 0x00;

    if ( argc <= 2){
        printf("\nUsage:\n");
        printf("\nykush -d downstream_number\n");
        printf("\nykush -u downstream_number\n");
        return 0;
    }

    
    if(argv[1][0]=='-' && argv[1][1]=='d') {
        switch(argv[2][0]) {
            case '1':
                // Downstream 1 down
                cmd = 0x01;
                command(cmd);
                break;
                
            case '2':
                // Downstream 2 down
                cmd = 0x02;
                command(cmd);
                break;
                
            case '3':
                // Downstream 3 down
                cmd = 0x03;
                command(cmd);
                break;
                
            case 'a':
                // All downstreams down
                cmd = 0x0a;
                command(cmd);
                break;

            case 'A':
                // MULTIBOARD DOWN
                if (argv[2][1] == '1') {
                    cmd = 0x01;
                    commandMultiBoard(0x0A42,cmd);
                    break;
                }
                if (argv[2][1] == '2') {
                    cmd = 0x02;
                    commandMultiBoard(0x0A42,cmd);
                    break;
                }
                if (argv[2][1] == '3') {
                    cmd = 0x03;
                    commandMultiBoard(0x0A42,cmd);
                    break;
                }
                break;

            case 'B':
                // MULTIBOARD DOWN
                if (argv[2][1] == '1') {
                    cmd = 0x01;
                    commandMultiBoard(0x0B42,cmd);
                    break;
                }
                if (argv[2][1] == '2') {
                    cmd = 0x02;
                    commandMultiBoard(0x0B42,cmd);
                    break;
                }
                if (argv[2][1] == '3') {
                    cmd = 0x03;
                    commandMultiBoard(0x0B42,cmd);
                    break;
                }
                break;

            case 'C':
                // MULTIBOARD DOWN
                if (argv[2][1] == '1') {
                    cmd = 0x01;
                    commandMultiBoard(0x0C42,cmd);
                    break;
                }
                if (argv[2][1] == '2') {
                    cmd = 0x02;
                    commandMultiBoard(0x0C42,cmd);
                    break;
                }
                if (argv[2][1] == '3') {
                    cmd = 0x03;
                    commandMultiBoard(0x0C42,cmd);
                    break;
                }
                break;

            case 'D':
                // MULTIBOARD DOWN
                if (argv[2][1] == '1') {
                    cmd = 0x01;
                    commandMultiBoard(0x0D42,cmd);
                    break;
                }
                if (argv[2][1] == '2') {
                    cmd = 0x02;
                    commandMultiBoard(0x0D42,cmd);
                    break;
                }
                if (argv[2][1] == '3') {
                    cmd = 0x03;
                    commandMultiBoard(0x0D42,cmd);
                    break;
                }
                break;


            default:
                printf("\nUsage:\n");
                printf("\nykush -d downstream_number\n");
                printf("\nykush -u downstream_number\n");
                break;
        }
    } else if(argv[1][0]=='-' && argv[1][1]=='u') {
        switch(argv[2][0]) {
            case '1':
                // Downstream 1 down
                cmd = 0x11;
                command(cmd);
                break;
                
            case '2':
                // Downstream 2 down
                cmd = 0x12;
                command(cmd);
                break;
                
            case '3':
                // Downstream 3 down
                cmd = 0x13;
                command(cmd);
                break;
                
            case 'a':
                // All downstreams down
                cmd = 0x1a;
                command(cmd);
                break;

            case 'A':
                // MULTIBOARD DOWN
                if (argv[2][1] == '1') {
                    cmd = 0x11;
                    commandMultiBoard(0x0A42,cmd);
                    break;
                }
                if (argv[2][1] == '2') {
                    cmd = 0x12;
                    commandMultiBoard(0x0A42,cmd);
                    break;
                }
                if (argv[2][1] == '3') {
                    cmd = 0x13;
                    commandMultiBoard(0x0A42,cmd);
                    break;
                }
                break;

            case 'B':
                // MULTIBOARD DOWN
                if (argv[2][1] == '1') {
                    cmd = 0x11;
                    commandMultiBoard(0x0B42,cmd);
                    break;
                }
                if (argv[2][1] == '2') {
                    cmd = 0x12;
                    commandMultiBoard(0x0B42,cmd);
                    break;
                }
                if (argv[2][1] == '3') {
                    cmd = 0x13;
                    commandMultiBoard(0x0B42,cmd);
                    break;
                }
                break;

            case 'C':
                // MULTIBOARD DOWN
                if (argv[2][1] == '1') {
                    cmd = 0x11;
                    commandMultiBoard(0x0C42,cmd);
                    break;
                }
                if (argv[2][1] == '2') {
                    cmd = 0x12;
                    commandMultiBoard(0x0C42,cmd);
                    break;
                }
                if (argv[2][1] == '3') {
                    cmd = 0x13;
                    commandMultiBoard(0x0C42,cmd);
                    break;
                }
                break;

            case 'D':
                // MULTIBOARD DOWN
                if (argv[2][1] == '1') {
                    cmd = 0x11;
                    commandMultiBoard(0x0D42,cmd);
                    break;
                }
                if (argv[2][1] == '2') {
                    cmd = 0x12;
                    commandMultiBoard(0x0D42,cmd);
                    break;
                }
                if (argv[2][1] == '3') {
                    cmd = 0x13;
                    commandMultiBoard(0x0D42,cmd);
                    break;
                }
                break;
                
            default:
                printf("\nUsage:\n");
                printf("\nykush -d downstream_number\n");
                printf("\nykush -u downstream_number\n");
                break;
        } 
    } else {
        printf("\nUsage:\n");
        printf("\nykush -d downstream_number\n");
        printf("\nykush -u downstream_number\n");
    }
        
 
    return 0;
}

