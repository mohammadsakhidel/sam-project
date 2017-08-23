package com.ramanco.samandroid.code.utils;

import android.app.Activity;
import android.content.Context;
import android.widget.Toast;

public class ExceptionManager {
    public static void Handle(final Activity activity, final Exception ex) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Toast.makeText(activity, String.format("EXCEPTION MANAGER:\n%s", ex.getMessage()), Toast.LENGTH_SHORT).show();
            }
        });
    }

    public static void Handle(final Context context, final Exception ex) {
        Toast.makeText(context, String.format("EXCEPTION MANAGER:\n%s", ex.getMessage()), Toast.LENGTH_SHORT).show();
    }

    public static void HandleListException(Context context, Exception ex) {
        Toast.makeText(context, String.format("EXCEPTION IN ADAPTER GET VIEW:\n%s", ex.getMessage()), Toast.LENGTH_SHORT).show();
    }
}
