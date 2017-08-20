package com.ramanco.samandroid.code.utils;

import java.lang.reflect.Field;
import android.content.Context;
import android.graphics.Typeface;
import android.view.View;
import android.widget.TextView;

public final class FontUtility {

    public static void overrideFont(Context context, String staticTypefaceFieldName,
                                      String fontAssetName) throws Exception {

        final Typeface regular = Typeface.createFromAsset(context.getAssets(),
                "fonts/" + fontAssetName);
        replaceFont(staticTypefaceFieldName, regular);

    }

    protected static void replaceFont(String staticTypefaceFieldName,
                                      final Typeface newTypeface) throws Exception {

        final Field staticField = Typeface.class.getDeclaredField(staticTypefaceFieldName);
        staticField.setAccessible(true);
        staticField.set(null, newTypeface);

    }

    public static void setViewFont(Context context, View view, String fontNameInAssets) {

        Typeface typeface = Typeface.createFromAsset(context.getAssets(), "fonts/" + fontNameInAssets);

        if (view instanceof TextView)
            ((TextView) view).setTypeface(typeface);

    }

}
