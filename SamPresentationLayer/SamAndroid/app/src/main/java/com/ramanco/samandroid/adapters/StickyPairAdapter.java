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
import com.ramanco.samandroid.objects.StickyHeader;
import com.ramanco.samandroid.utils.ExceptionManager;

import java.util.Collections;
import java.util.List;

import se.emilsjolander.stickylistheaders.StickyListHeadersAdapter;

public class StickyPairAdapter extends ArrayAdapter<KeyValuePair> implements StickyListHeadersAdapter {

    //region Fields:
    Context context;
    KeyValuePair[] items;
    List<StickyHeader> stickyHeaders;
    //endregion

    //region Ctors:
    public StickyPairAdapter(Context context, KeyValuePair[] objects, List<StickyHeader> stickyHeaders) {
        super(context, -1, objects);

        this.context = context;
        this.items = objects;
        this.stickyHeaders = stickyHeaders;
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

    //region StickyListHeadersAdapter Implementation:
    @Override
    public View getHeaderView(int position, View convertView, ViewGroup parent) {

        if (convertView == null) {
            LayoutInflater inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            convertView = inflater.inflate(R.layout.list_item_sticky_header, parent, false);
        }

        StickyHeader section = stickyHeaders.get(findSectionIndex(position));
        String headerText = section.getTitle();
        TextView txtTitle = (TextView) convertView.findViewById(R.id.tv_text);
        txtTitle.setTextColor(section.getTextColor());
        txtTitle.setText(headerText);

        convertView.setBackgroundColor(section.getBgColor());

        return convertView;

    }

    @Override
    public long getHeaderId(int position) {
        if (stickyHeaders.size() > 1) {
            return findSectionIndex(position);
        } else {
            return 728;
        }
    }
    //endregion

    //region Methods:
    private int findSectionIndex(int position) {
        int index = stickyHeaders.size() - 1;

        Collections.sort(stickyHeaders);
        for (int i = 0; i < stickyHeaders.size() - 1; i++) {
            if (position >= stickyHeaders.get(i).getStartIndex()
                    && position < stickyHeaders.get(i + 1).getStartIndex())
                return i;
        }

        return index;
    }
    //endregion
}
