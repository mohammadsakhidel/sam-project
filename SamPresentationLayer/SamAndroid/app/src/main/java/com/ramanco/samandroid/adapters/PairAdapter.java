package com.ramanco.samandroid.adapters;

import android.content.Context;
import android.util.Pair;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.code.objects.KeyValuePair;
import com.ramanco.samandroid.code.utils.ExceptionManager;

public class PairAdapter extends ArrayAdapter<KeyValuePair> {

    //region Fields:
    Context context;
    KeyValuePair[] items;
    //endregion

    //region Ctors:
    public PairAdapter(Context context, KeyValuePair[] objects) {
        super(context, -1, objects);

        this.context = context;
        this.items = objects;
    }
    //endregion

    //region Overrides:
    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        if (convertView == null) {
            LayoutInflater inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            convertView = inflater.inflate(R.layout.list_item_pair, parent, false);
        }

        try {

            TextView tv_text = (TextView) convertView.findViewById(R.id.tv_text);
            tv_text.setText(items[position].getValue());

        } catch (Exception ex) {
            ExceptionManager.HandleListException(context, ex);
        }

        return convertView;
    }
    //endregion
}
