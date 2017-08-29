package com.ramanco.samandroid.adapters;

import android.app.Activity;
import android.app.ProgressDialog;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.drawable.Drawable;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.activities.BaseActivity;
import com.ramanco.samandroid.api.dtos.TemplateDto;
import com.ramanco.samandroid.consts.ApiActions;
import com.ramanco.samandroid.consts.Configs;
import com.ramanco.samandroid.fragments.ImageViewerFragment;
import com.ramanco.samandroid.objects.KeyValuePair;
import com.ramanco.samandroid.objects.StickyHeader;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.UxUtil;
import com.squareup.picasso.Picasso;
import com.squareup.picasso.Target;

import java.net.URL;
import java.util.Collections;
import java.util.List;
import java.util.Locale;

import retrofit2.http.Url;
import se.emilsjolander.stickylistheaders.StickyListHeadersAdapter;

public class TemplateAdapter extends ArrayAdapter<TemplateDto> implements StickyListHeadersAdapter {

    //region Fields:
    Context context;
    TemplateDto[] items;
    List<StickyHeader> stickyHeaders;
    Target target;
    ProgressDialog progress;
    //endregion

    //region Ctors:
    public TemplateAdapter(final Context context, TemplateDto[] objects, List<StickyHeader> stickyHeaders) {
        super(context, -1, objects);

        this.context = context;
        this.items = objects;
        this.stickyHeaders = stickyHeaders;
        this.target = new Target() {
            @Override
            public void onBitmapLoaded(Bitmap bitmap, Picasso.LoadedFrom from) {
                try {
                    if (progress != null)
                        progress.dismiss();

                    showImageViewFragment(bitmap);

                } catch (Exception ex) {
                    if (progress != null)
                        progress.dismiss();
                    ExceptionManager.Handle(context, ex);
                }
            }

            @Override
            public void onBitmapFailed(Drawable errorDrawable) {
                try {
                    if (progress != null)
                        progress.dismiss();
                    Toast.makeText(context,
                            context.getString(R.string.operation_failed),
                            Toast.LENGTH_SHORT).show();
                } catch (Exception ex) {
                    if (progress != null)
                        progress.dismiss();
                    ExceptionManager.Handle(context, ex);
                }
            }

            @Override
            public void onPrepareLoad(Drawable placeHolderDrawable) {

            }
        };
    }
    //endregion

    //region Overrides:
    @Override
    public View getView(int position, View convertView, ViewGroup parent) {

        try {

            final TemplateDto template = items[position];
            if (convertView == null) {
                LayoutInflater inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
                convertView = inflater.inflate(R.layout.list_item_template, parent, false);
            }

            //region Icon ImageView:
            ImageView imgIcon = (ImageView) convertView.findViewById(R.id.img_icon);
            URL iconUrl = new URL(new URL(Configs.API_BASE_ADDRESS),
                    String.format("%s/%s?thumb=true", ApiActions.blobs_getimage, template.getBackgroundImageID()));
            Picasso.with(context).load(iconUrl.toString()).into(imgIcon);
            imgIcon.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    try {
                        //region show template preview:
                        URL imageUrl = new URL(new URL(Configs.API_BASE_ADDRESS),
                                String.format("%s/%s?thumb=false", ApiActions.blobs_getimage, template.getBackgroundImageID()));
                        loadTemplateImage(imageUrl.toString());
                        //endregion
                    } catch (Exception ex) {
                        ExceptionManager.Handle(context, ex);
                    }
                }
            });
            //endregion

            //region Title TextView:
            TextView tv_text = (TextView) convertView.findViewById(R.id.tv_text);
            tv_text.setText(template.getName());
            //endregion

            //region Price TextView:
            TextView tv_desc = (TextView) convertView.findViewById(R.id.tv_desc);
            tv_desc.setText(String.format(Locale.US, "%s: %,d %s",
                    context.getResources().getString(R.string.price),
                    (int) template.getPrice(),
                    context.getResources().getString(R.string.price_unit)));
            //endregion

        } catch (Exception ex) {
            ExceptionManager.HandleListException(context, ex);
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

    private void loadTemplateImage(String remoteUrl) {
        progress = UxUtil.showProgress(context);
        Picasso.with(context).load(remoteUrl).into(target);
    }

    private void showImageViewFragment(Bitmap bitmap) {
        ImageViewerFragment imageViewerFragment = new ImageViewerFragment();
        imageViewerFragment.setBitmap(bitmap);

        FragmentManager fm = ((BaseActivity) context).getSupportFragmentManager();
        FragmentTransaction ft = fm.beginTransaction();
        ft.replace(R.id.ly_image_viewer_fragment_placeholder, imageViewerFragment);
        ft.setTransition(FragmentTransaction.TRANSIT_FRAGMENT_OPEN);
        ft.addToBackStack(null);
        ft.commit();
    }
    //endregion
}
