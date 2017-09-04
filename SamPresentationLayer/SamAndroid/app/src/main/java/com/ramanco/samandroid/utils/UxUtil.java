package com.ramanco.samandroid.utils;

import android.app.ProgressDialog;
import android.content.Context;

import com.ramanco.samandroid.R;

public class UxUtil {
    public  static ProgressDialog showProgress(Context context) {

        ProgressDialog progress =  new ProgressDialog(context);
        progress.setProgressStyle(ProgressDialog.STYLE_SPINNER);
        //progress.setTitle(context.getResources().getString(R.string.connecting_server));
        progress.setMessage(context.getResources().getString(R.string.please_wait));
        progress.setIndeterminate(true);
        progress.show();

        return progress;
    }
}
