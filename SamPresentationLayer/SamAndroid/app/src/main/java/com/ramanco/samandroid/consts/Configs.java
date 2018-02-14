package com.ramanco.samandroid.consts;

import com.ramanco.samandroid.api.ApiKeyGenerator3;
import com.ramanco.samandroid.utils.ApiKeyGenerator1;
import com.ramanco.samandroid.utils.TextUtility;

public class Configs {

    public static String getApiBaseAddress() {
        try {
            String str = TextUtility.bytesToString(ApiKeyGenerator3.getApiBaseAddressBytes());
            return TextUtility.fromBase64(str);
        } catch (Exception e) {
            return "";
        }
    }

    public static String getAuthHeaderName() {
        try {
            String str = TextUtility.bytesToString(ApiKeyGenerator2.getHeaderNameBytes());
            return TextUtility.fromBase64(str);
        } catch (Exception e) {
            return "";
        }
    }

    public static String getAuthToken() {
        try {
            String sb = TextUtility.bytesToString(ApiKeyGenerator1.getPart1Bytes()) +
                    TextUtility.bytesToString(ApiKeyGenerator2.getPart2Bytes()) +
                    TextUtility.bytesToString(ApiKeyGenerator3.getPart3Bytes());

            return TextUtility.fromBase64(sb);
        } catch (Exception e) {
            return "";
        }
    }

}
