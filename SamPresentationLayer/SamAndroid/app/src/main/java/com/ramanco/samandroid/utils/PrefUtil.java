package com.ramanco.samandroid.utils;

import android.content.Context;
import android.content.SharedPreferences;

public class PrefUtil {

    //region ARGS:
    public static final String PREF_MAIN = "main_preferences";
    public static final String PREF_CITY_ID = "pref_city_id";
    public static final String PREF_CUSTOMER_INFO = "pref_customer_info";
    //endregion

    //region GET methods:
    public static int getCityID(Context context) {
        SharedPreferences preferences = context.getSharedPreferences(PREF_MAIN, Context.MODE_PRIVATE);
        if (preferences != null) {
            return preferences.getInt(PREF_CITY_ID, -1);
        }
        return -1;
    }

    public static String getCustomerInfo(Context context) {
        SharedPreferences preferences = context.getSharedPreferences(PREF_MAIN, Context.MODE_PRIVATE);
        if (preferences != null) {
            return preferences.getString(PREF_CUSTOMER_INFO, "");
        }
        return "";
    }
    //endregion

    //region SET methods:
    public static void setCityID(Context context, int cityId) {
        SharedPreferences preferences = context.getSharedPreferences(PREF_MAIN, Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = preferences.edit();
        editor.putInt(PREF_CITY_ID, cityId);
        editor.apply();
    }

    public static void setCustomerInfo(Context context, String customerInfoJson) {
        SharedPreferences preferences = context.getSharedPreferences(PREF_MAIN, Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = preferences.edit();
        editor.putString(PREF_CUSTOMER_INFO, customerInfoJson);
        editor.apply();
    }
    //endregion
}
