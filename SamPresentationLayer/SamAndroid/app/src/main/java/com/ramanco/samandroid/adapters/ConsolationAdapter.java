package com.ramanco.samandroid.adapters;

import android.app.ProgressDialog;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.drawable.Drawable;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.activities.BaseActivity;
import com.ramanco.samandroid.api.dtos.ConsolationDto;
import com.ramanco.samandroid.consts.ApiActions;
import com.ramanco.samandroid.consts.Configs;
import com.ramanco.samandroid.enums.ConsolationStatus;
import com.ramanco.samandroid.enums.PaymentStatus;
import com.ramanco.samandroid.fragments.ImageViewerFragment;
import com.ramanco.samandroid.objects.StickyHeader;
import com.ramanco.samandroid.utils.EnumUtil;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.UxUtil;
import com.squareup.picasso.Picasso;
import com.squareup.picasso.Target;

import java.net.URL;
import java.util.Collections;
import java.util.Locale;

import se.emilsjolander.stickylistheaders.StickyListHeadersAdapter;

public class ConsolationAdapter extends ArrayAdapter<ConsolationDto> {

    //region Fields:
    Context context;
    ConsolationDto[] items;
    Target target;
    ProgressDialog progress;
    //endregion

    //region Ctors:
    public ConsolationAdapter(final Context context, ConsolationDto[] objects) {
        super(context, -1, objects);

        this.context = context;
        this.items = objects;
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
                    ExceptionManager.handle(context, ex);
                }
            }

            @Override
            public void onBitmapFailed(Drawable errorDrawable) {
                try {
                    if (progress != null)
                        progress.dismiss();
                    Toast.makeText(context,
                            context.getString(R.string.msg_loading_image_failed),
                            Toast.LENGTH_SHORT).show();
                } catch (Exception ex) {
                    if (progress != null)
                        progress.dismiss();
                    ExceptionManager.handle(context, ex);
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

            if (convertView == null) {
                LayoutInflater inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
                convertView = inflater.inflate(R.layout.list_item_consolation, parent, false);
            }

            final ConsolationDto consolation = items[position];

            //region Icon ImageView:
            ImageView imgIcon = (ImageView) convertView.findViewById(R.id.img_icon);
            URL iconUrl = new URL(new URL(Configs.API_BASE_ADDRESS),
                    String.format("%s/%s?thumb=true", ApiActions.consolations_getpreview, consolation.getId()));
            Picasso.with(context).load(iconUrl.toString()).into(imgIcon);
            imgIcon.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    try {
                        //region show template preview:
                        URL imageUrl = new URL(new URL(Configs.API_BASE_ADDRESS),
                                String.format("%s/%s?thumb=false", ApiActions.consolations_getpreview, consolation.getId()));
                        loadConsolationPreview(imageUrl.toString());
                        //endregion
                    } catch (Exception ex) {
                        ExceptionManager.handle(context, ex);
                    }
                }
            });
            //endregion

            //region Title TextView:
            TextView tvText = (TextView) convertView.findViewById(R.id.tv_text);
            tvText.setText(String.format("%s: %s",
                    context.getResources().getString(R.string.obit),
                    consolation.getObit().getTitle()));
            //endregion

            //region Status TextView:
            TextView tvStatus = (TextView) convertView.findViewById(R.id.tv_status);
            tvStatus.setText(String.format(Locale.US, "%s: %s",
                    context.getResources().getString(R.string.status),
                    EnumUtil.getConsolationStatusText(context, ConsolationStatus.valueOf(consolation.getStatus()))));
            tvStatus.setTextColor(EnumUtil.getConsolationStatusColor(context, ConsolationStatus.valueOf(consolation.getStatus())));
            //endregion

            //region Payment TextView:
            TextView tvPayment = (TextView) convertView.findViewById(R.id.tv_payment);
            tvPayment.setText(String.format(Locale.US, "%s: %,d %s (%s)",
                    context.getResources().getString(R.string.cost),
                    (int) consolation.getTemplate().getPrice(),
                    context.getResources().getString(R.string.price_unit),
                    EnumUtil.getPaymentStatusText(context, PaymentStatus.valueOf(consolation.getPaymentStatus()))));
            tvPayment.setTextColor(EnumUtil.getPaymentStatusColor(context, PaymentStatus.valueOf(consolation.getPaymentStatus())));
            //endregion

        } catch (Exception ex) {
            ExceptionManager.handleListException(context, ex);
        }

        return convertView;
    }
    //endregion

    //region Methods:
    private void loadConsolationPreview(String remoteUrl) {
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
