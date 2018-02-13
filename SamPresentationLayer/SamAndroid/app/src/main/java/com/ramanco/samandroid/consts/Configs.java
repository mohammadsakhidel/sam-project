package com.ramanco.samandroid.consts;

import com.ramanco.samandroid.utils.TextUtility;

public class Configs {
    public static final String API_BASE_ADDRESS = "https://api.samsys.ir/";

    public static String getAuthHeaderName() {
        try {
            return TextUtility.fromBase64("QXV0aG9yaXphdGlvbg==");
        } catch (Exception e) {
            return "";
        }
    }

    public static String getAuthToken() {
        try {
            return TextUtility.fromBase64("QmFzaWMgYzNKdlUxWnhSbkU1V1RweVRGWllOMUJEWkZSWGJsbENlblZHTlZRNWNYSlFRMVE1YldWU1ZqSjNUQT09");
        } catch (Exception e) {
            return "";
        }
    }
}
