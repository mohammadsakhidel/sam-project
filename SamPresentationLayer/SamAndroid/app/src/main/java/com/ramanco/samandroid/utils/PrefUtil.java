package com.ramanco.samandroid.utils;

import android.content.Context;
import android.content.SharedPreferences;

public class PrefUtil {

    //region ARGS:
    public static final String PREF_MAIN = "main_preferences";
    public static final String PREF_CITY_ID = "pref_city_id";
    //endregion

    //region GET methods:
    public static int getCityID(Context context) {
        SharedPreferences preferences = context.getSharedPreferences(PREF_MAIN, Context.MODE_PRIVATE);
        if (preferences != null) {
            return preferences.getInt(PREF_CITY_ID, -1);
        }
        return -1;
    }
    //endregion

    //region SET methods:
    public static void setCityID(Context context, int cityId) {
        SharedPreferences preferences = context.getSharedPreferences(PREF_MAIN, Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = preferences.edit();
        editor.putInt(PREF_CITY_ID, cityId);
        editor.apply();
    }
    //endregion
}
