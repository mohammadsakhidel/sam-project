package com.ramanco.samandroid.consts;

public class Patterns {
    public static final String NON_ALPHANUMERIC_CHARS = "[_+\\-.,!@#$%^&*();\\\\/|<>\"']";
    public static final String SPACE_CHARS = "\\s+";
    public static final String PASSWORD = "^.{6,}$";
    public static final String EMAIL = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
    public static final String CELLPHONE = "^0?9[0-9]{9}$";
    public static final String STRING_LIST__SEP = "\\s*,\\s*";
}
