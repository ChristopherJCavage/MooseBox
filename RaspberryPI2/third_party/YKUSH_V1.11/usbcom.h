#ifndef USBCOM_H
#define USBCOM_H

static int sendCommand(unsigned char *toSendBuffer, unsigned char *receivedBuffer);

int command(char cmd);

int commandMultiBoard(int pid, char cmd);




#endif // USBCOM_H
