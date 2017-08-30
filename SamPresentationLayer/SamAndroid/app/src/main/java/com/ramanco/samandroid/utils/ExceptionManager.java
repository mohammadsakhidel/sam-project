package com.ramanco.samandroid.utils;

import android.app.Activity;
import android.content.Context;
import android.widget.Toast;

public class ExceptionManager {
    public static void handle(final Activity activity, final Exception ex) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Toast.makeText(activity, String.format("EXCEPTION MANAGER:\n%s", ex.getMessage()), Toast.LENGTH_LONG).show();
            }
        });
    }

    public static void handle(final Context context, final Exception ex) {
        Toast.makeText(context, String.format("EXCEPTION MANAGER:\n%s", ex.getMessage()), Toast.LENGTH_LONG).show();
    }

    public static void handleListException(Context context, Exception ex) {
        Toast.makeText(context, String.format("EXCEPTION IN ADAPTER GET VIEW:\n%s", ex.getMessage()), Toast.LENGTH_LONG).show();
    }
}
