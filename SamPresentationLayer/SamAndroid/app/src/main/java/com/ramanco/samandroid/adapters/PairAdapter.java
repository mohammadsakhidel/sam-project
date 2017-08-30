package com.ramanco.samandroid.adapters;

import android.content.Context;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.objects.KeyValuePair;
import com.ramanco.samandroid.utils.ExceptionManager;

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

        try {

            KeyValuePair pair = items[position];
            if (convertView == null) {
                LayoutInflater inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
                if (TextUtils.isEmpty(pair.getDesc()))
                    convertView = inflater.inflate(R.layout.list_item_simple, parent, false);
                else
                    convertView = inflater.inflate(R.layout.list_item_simple_with_desc, parent, false);
            }

            TextView tv_text = (TextView) convertView.findViewById(R.id.tv_text);
            tv_text.setText(items[position].getValue());

            if (!TextUtils.isEmpty(pair.getDesc())) {
                TextView tv_desc = (TextView) convertView.findViewById(R.id.tv_desc);
                tv_desc.setText(items[position].getDesc());
                tv_desc.setVisibility(View.VISIBLE);
            }

        } catch (Exception ex) {
            ExceptionManager.handleListException(context, ex);
        }

        return convertView;
    }
    //endregion
}
