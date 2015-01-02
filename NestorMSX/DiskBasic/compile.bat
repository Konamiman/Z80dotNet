@echo off
cls
for %%A in (CODES,DSKBASIC) do cpm32 M80 =%%A
cpm32 L80 /P:4000,CODES,DSKBASIC,dskrom/N/X/Y/E
hex2bin -s 4000 dskrom.hex
copy dskrom.bin ..\dskrom.rom

 