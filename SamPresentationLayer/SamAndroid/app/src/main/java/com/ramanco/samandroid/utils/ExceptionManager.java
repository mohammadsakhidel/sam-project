package com.ramanco.samandroid.utils;

import android.app.Activity;
import android.content.Context;
import android.widget.Toast;

import com.ramanco.samandroid.R;

import java.net.UnknownHostException;

public class ExceptionManager {

    public static void handle(final Activity activity, final Exception ex) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                handleException(activity, ex);
            }
        });
    }
    public static void handle(final Context context, final Exception ex) {
        handleException(context, ex);
    }
    public static void handleListException(Context context, Exception ex) {
        handleException(context, ex);
    }
    private static void handleException(Context context, Exception ex) {
        if (ex instanceof UnknownHostException) {
            Toast.makeText(context, context.getString(R.string.msg_call_server_failed),
                    Toast.LENGTH_LONG).show();
        } else {
            Toast.makeText(context, context.getString(R.string.msg_general_error_message),
                    Toast.LENGTH_LONG).show();
        }
    }

}
