package com.ramanco.samandroid.exceptions;

import android.content.Context;

import com.ramanco.samandroid.R;

public class CallServerException extends Exception {
    public CallServerException(String message) {
        super(message);
    }

    public CallServerException(Context context) {
        super(context.getResources().getString(R.string.msg_call_server_failed));
    }
}
