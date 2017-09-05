package com.ramanco.samandroid.utils;

import android.content.Context;
import android.content.SharedPreferences;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.ramanco.samandroid.consts.Patterns;

import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Map;

public class PrefUtil {

    //region ARGS:
    public static final String PREF_MAIN = "main_preferences";
    public static final String PREF_CITY_ID = "pref_city_id";
    public static final String PREF_CUSTOMER_INFO = "pref_customer_info";
    public static final String PREF_TRACKING_NUMS = "pref_tracking_nums";
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

    public static List<String> getTrackingNumbers(Context context) {
        SharedPreferences preferences = context.getSharedPreferences(PREF_MAIN, Context.MODE_PRIVATE);
        if (preferences != null) {
            String listString = preferences.getString(PREF_TRACKING_NUMS, "");
            String[] array = listString.split(Patterns.STRING_LIST__SEP);
            return Arrays.asList(array);
        }
        return null;
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

    public static void setTrackingNumbers(Context context, List<String> trackingNumbers) {
        SharedPreferences preferences = context.getSharedPreferences(PREF_MAIN, Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = preferences.edit();
        String listString = TextUtility.concat(trackingNumbers.toArray(new String[trackingNumbers.size()]), ',', false);
        editor.putString(PREF_TRACKING_NUMS, listString);
        editor.apply();
    }
    //endregion
}
