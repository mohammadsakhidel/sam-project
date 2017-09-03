package com.ramanco.samandroid.fragments;


import android.app.ProgressDialog;
import android.graphics.Bitmap;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.activities.BaseActivity;
import com.ramanco.samandroid.api.endpoints.ConsolationsApiEndpoint;
import com.ramanco.samandroid.consts.ApiActions;
import com.ramanco.samandroid.consts.Configs;
import com.ramanco.samandroid.exceptions.ImageLoadingException;
import com.ramanco.samandroid.utils.ApiUtil;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.UxUtil;
import com.squareup.picasso.Picasso;
import com.squareup.picasso.Target;

import java.net.MalformedURLException;
import java.net.URL;

public class PreviewStepFragment extends Fragment {

    //region Fields:
    SendConsolationFragment parentView;
    Bitmap previewBitmap;
    Target target;
    //endregion

    //region Ctors:
    public PreviewStepFragment() {
    }
    //endregion

    //region Overrides:
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View fragmentView = inflater.inflate(R.layout.fragment_preview_step, container, false);

        try {

            loadPreviewImageAsync(fragmentView);

        } catch (Exception ex) {
            ExceptionManager.handle(getActivity(), ex);
        }

        return fragmentView;
    }
    //endregion

    //region Getters & Setters:
    public SendConsolationFragment getParentView() {
        return parentView;
    }

    public void setParentView(SendConsolationFragment parentView) {
        this.parentView = parentView;
    }
    //endregion

    //region Methods:
    private void loadPreviewImageAsync(final View fragmentView) throws MalformedURLException {
        URL url = new URL(new URL(Configs.API_BASE_ADDRESS), String.format("%s/%s", ApiActions.consolations_getpreview,
                Integer.toString(parentView.getCreatedConsolationId())));
        final ProgressDialog progress = UxUtil.showProgress(getActivity());

        //region create target:
        target = new Target() {
            @Override
            public void onBitmapLoaded(final Bitmap bitmap, Picasso.LoadedFrom from) {
                try {
                    previewBitmap = bitmap;
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            try {
                                progress.dismiss();
                                ImageView imgPreview = (ImageView) fragmentView.findViewById(R.id.img_preview);
                                imgPreview.setImageBitmap(bitmap);
                                //region zoom on click:
                                imgPreview.setOnClickListener(new View.OnClickListener() {
                                    @Override
                                    public void onClick(View v) {
                                        try {
                                            ImageViewerFragment imageViewerFragment = new ImageViewerFragment();
                                            imageViewerFragment.setBitmap(bitmap);

                                            FragmentManager fm = getActivity().getSupportFragmentManager();
                                            FragmentTransaction ft = fm.beginTransaction();
                                            ft.replace(R.id.ly_image_viewer_fragment_placeholder, imageViewerFragment);
                                            ft.setTransition(FragmentTransaction.TRANSIT_FRAGMENT_OPEN);
                                            ft.addToBackStack(null);
                                            ft.commit();
                                        } catch (Exception ex) {
                                            ExceptionManager.handle(getActivity(), ex);
                                        }
                                    }
                                });
                                //endregion
                            } catch (Exception ex) {
                                ExceptionManager.handle(getActivity(), ex);
                            }
                        }
                    });
                } catch (Exception ex) {
                    ExceptionManager.handle(getActivity(), ex);
                }
            }

            @Override
            public void onBitmapFailed(Drawable errorDrawable) {
                try {
                    //region dismiss progress:
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            try {
                                progress.dismiss();
                            } catch (Exception ex) {
                                ExceptionManager.handle(getActivity(), ex);
                            }
                        }
                    });
                    //endregion
                    throw new ImageLoadingException(getActivity().getResources().getString(R.string.msg_loading_image_failed));
                } catch (Exception ex) {
                    ExceptionManager.handle(getActivity(), ex);
                }
            }

            @Override
            public void onPrepareLoad(Drawable placeHolderDrawable) {
            }
        };
        //endregion

        Picasso.with(getActivity()).load(url.toString()).into(target);
    }
    //endregion

}
