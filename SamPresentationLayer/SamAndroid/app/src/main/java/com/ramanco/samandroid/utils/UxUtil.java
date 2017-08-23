package com.ramanco.samandroid.utils;

import android.app.ProgressDialog;
import android.content.Context;

import com.ramanco.samandroid.R;

public class UxUtil {
    public  static ProgressDialog showProgress(Context context) {
        return ProgressDialog.show(context,
                context.getResources().getString(R.string.connecting_server),
                context.getResources().getString(R.string.please_wait), true);
    }
}
