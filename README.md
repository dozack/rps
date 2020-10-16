# RPS Serial Port application

My school project for RPS class on Technical university of Liberec.

## Protocol

Frame structure: START-LEN-ADDR-DATA

-  START: frame start character, in this case '#', 1B
-  LEN: Frame length (0-9) in ASCII format (containg ADDR + DATA length), 1B
-  ADDR: Address of "register" to be updated (GUI controls could be mapped to this), 1B
-  DATA: Integer encoded as ASCII string, 1B * (LEN-1)

## Usage

1. Create virtual serial port loopback (for example com0com).
2. Open two instances of provided exe.
3. Check Server checkbox in one of the windows (this will be transmitter).
4. Connect to loopback serial ports in both windows.
5. Fiddle with trackbar and button in with checked "Server" box.
