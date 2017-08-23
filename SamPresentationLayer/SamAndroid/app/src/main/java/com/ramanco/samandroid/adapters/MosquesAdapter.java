package com.ramanco.samandroid.adapters;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.api.dtos.MosqueDto;
import com.ramanco.samandroid.utils.ExceptionManager;

public class MosquesAdapter extends ArrayAdapter<MosqueDto> {

    //region Fields:
    Context context;
    MosqueDto[] items;
    //endregion

    //region Ctors:
    public MosquesAdapter(Context context, MosqueDto[] objects) {
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
            convertView = inflater.inflate(R.layout.list_item_simple_with_desc, parent, false);
        }

        try {

            MosqueDto mosque = items[position];

            TextView tv_text = (TextView) convertView.findViewById(R.id.tv_text);
            tv_text.setText(mosque.getName());

            TextView tv_desc = (TextView) convertView.findViewById(R.id.tv_desc);
            tv_desc.setText(String.format("%s: %s",
                    context.getResources().getString(R.string.address),
                    mosque.getAddress()));

        } catch (Exception ex) {
            ExceptionManager.HandleListException(context, ex);
        }

        return convertView;
    }
    //endregion
}
