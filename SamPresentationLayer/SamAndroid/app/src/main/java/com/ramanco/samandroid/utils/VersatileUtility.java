package com.ramanco.samandroid.utils;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.util.DisplayMetrics;

import com.ramanco.samandroid.BuildConfig;

import java.security.SecureRandom;
import java.util.Date;
import java.util.List;
import java.util.Random;
import java.util.concurrent.ThreadLocalRandom;

public class VersatileUtility {

    public static int getRandomNumber(int includedMin, int excludedMax) {
        Random rand = new Random();
        return rand.nextInt(excludedMax - includedMin) + includedMin;
    }

    public static char getRandomChar() {
        Random r = new Random();
        return (char) (r.nextInt(26) + 'a');
    }

    public static String getRandomString(int length) {
        final String AB = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        SecureRandom rnd = new SecureRandom();

        StringBuilder sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
            sb.append(AB.charAt(rnd.nextInt(AB.length())));
        return sb.toString();
    }

    public static int toPixels(Context context, int dps) {
        DisplayMetrics displayMetrics = context.getResources().getDisplayMetrics();
        return Math.round(dps * (displayMetrics.xdpi / DisplayMetrics.DENSITY_DEFAULT));
    }

    public static int toDPs(Context context, int pixels) {
        DisplayMetrics displayMetrics = context.getResources().getDisplayMetrics();
        return Math.round(pixels / (displayMetrics.xdpi / DisplayMetrics.DENSITY_DEFAULT));
    }

    public static int getDisplayWidthDP(Context context) {
        DisplayMetrics displayMetrics = context.getResources().getDisplayMetrics();
        //float dpHeight = displayMetrics.heightPixels / displayMetrics.density;
        return (int) (displayMetrics.widthPixels / displayMetrics.density);
    }

    public static int getDisplayHeightDP(Context context) {
        DisplayMetrics displayMetrics = context.getResources().getDisplayMetrics();
        return (int) (displayMetrics.heightPixels / displayMetrics.density);
    }

    public static void restart(Context context) {
        Intent intent = context.getPackageManager().getLaunchIntentForPackage(context.getPackageName());
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        context.startActivity(intent);
    }

    public static boolean equals(Object a, Object b) {
        return (a == b) || (a != null && a.equals(b));
    }

    public static String toLatinDigits(String input) {
        return input
                .replace('۰', '0')
                .replace('۱', '1')
                .replace('۲', '2')
                .replace('۳', '3')
                .replace('۴', '4').replace('٤', '4')
                .replace('۵', '5')
                .replace('۶', '6').replace('٦', '6')
                .replace('۷', '7')
                .replace('۸', '8')
                .replace('۹', '9');
    }

}
