package com.ramanco.samandroid.consts;

class ApiKeyGenerator2 {
    static byte[] getHeaderNameBytes() {
        return new byte[] {
                0x51, 0x58, 0x56, 0x30, 0x61, 0x47, 0x39, 0x79, 0x61, 0x58,
                0x70, 0x68, 0x64, 0x47, 0x6c, 0x76, 0x62, 0x67, 0x3d, 0x3d
        };
    }

    static byte[] getPart2Bytes() {
        return new byte[]{
                0b01100010, 0b01101011, 0b01010101, 0b00110001, 0b01010110, 0b00110001, 0b01010010,
                0b01110111, 0b01100101, 0b01010110, 0b01010010, 0b01000111, 0b01010111, 0b01101100, 0b01101100,
                0b01001111, 0b01001101, 0b01010101, 0b01001010, 0b01000101, 0b01010111, 0b01101011, 0b01011010,
                0b01010011, 0b01010111, 0b01000111, 0b01001010, 0b01110011, 0b01100010, 0b01000101
        };
    }
}
