package com.ramanco.samandroid.utils;

import android.content.Context;
import android.support.v4.content.ContextCompat;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.enums.ConsolationStatus;
import com.ramanco.samandroid.enums.ObitType;
import com.ramanco.samandroid.enums.PaymentStatus;

public class EnumUtil {

    public static String getPaymentStatusText(Context context, PaymentStatus paymentStatus) {
        switch (paymentStatus) {
            case failed:
                return context.getResources().getString(R.string.payment_failed);
            case succeeded:
                return context.getResources().getString(R.string.payment_succeeded);
            case pending:
                return context.getResources().getString(R.string.payment_pending);
        }
        return "";
    }
    public static int getPaymentStatusColor(Context context, PaymentStatus paymentStatus) {
        switch (paymentStatus) {
            case failed:
                return ContextCompat.getColor(context, R.color.colorStatusFailed);
            case succeeded:
                return ContextCompat.getColor(context, R.color.colorStatusOK);
            case pending:
                return ContextCompat.getColor(context, R.color.colorStatusPending);
        }
        return ContextCompat.getColor(context, R.color.colorTextPrimary);
    }

    public static String getConsolationStatusText(Context context, ConsolationStatus consolationStatus) {
        switch (consolationStatus) {
            case canceled:
                return context.getResources().getString(R.string.consolation_canceled);
            case displayed:
                return context.getResources().getString(R.string.consolation_displayed);
            case pending:
                return context.getResources().getString(R.string.consolation_pending);
            case confirmed:
                return context.getResources().getString(R.string.consolation_confirmed);
        }
        return "";
    }
    public static int getConsolationStatusColor(Context context, ConsolationStatus consolationStatus) {
        switch (consolationStatus) {
            case canceled:
                return ContextCompat.getColor(context, R.color.colorStatusFailed);
            case displayed:
                return ContextCompat.getColor(context, R.color.colorStatusOKDark);
            case pending:
                return ContextCompat.getColor(context, R.color.colorStatusPending);
            case confirmed:
                return ContextCompat.getColor(context, R.color.colorStatusOK);
        }
        return ContextCompat.getColor(context, R.color.colorTextPrimary);
    }

    public static String getObitTypeText(Context context, ObitType obitType) {
        switch (obitType) {
            case chehelom:
                return context.getResources().getString(R.string.obit_type_chehelom);
            case haftom:
                return context.getResources().getString(R.string.obit_type_haftom);
            case salgard:
                return context.getResources().getString(R.string.obit_type_salgard);
            case sevom:
                return context.getResources().getString(R.string.obit_type_sevom);
            case tarhim:
                return context.getResources().getString(R.string.obit_type_tarhim);
            case yadbood:
                return context.getResources().getString(R.string.obit_type_yadbood);
            case sayer:
                return context.getResources().getString(R.string.obit_type_sayer);
        }
        return "";
    }
}
