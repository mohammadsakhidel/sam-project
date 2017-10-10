package com.ramanco.samandroid.fragments;


import android.app.ProgressDialog;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.drawable.Drawable;
import android.net.Uri;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v4.app.NotificationCompat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.activities.BaseActivity;
import com.ramanco.samandroid.api.dtos.ConsolationDto;
import com.ramanco.samandroid.api.endpoints.ConsolationsApiEndpoint;
import com.ramanco.samandroid.consts.ApiActions;
import com.ramanco.samandroid.consts.Configs;
import com.ramanco.samandroid.enums.PaymentStatus;
import com.ramanco.samandroid.exceptions.CallServerException;
import com.ramanco.samandroid.exceptions.ImageLoadingException;
import com.ramanco.samandroid.utils.ApiUtil;
import com.ramanco.samandroid.utils.DateTimeUtility;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.UxUtil;
import com.squareup.picasso.Picasso;
import com.squareup.picasso.Target;

import org.joda.time.DateTimeUtils;

import java.net.MalformedURLException;
import java.net.URL;
import java.util.Locale;

import retrofit2.Call;
import retrofit2.Response;

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

            double amountToPay = parentView.getSelectedTemplate().getPrice();
            loadPreviewImageAsync(fragmentView);

            //region set description text:
            TextView tvDescription = (TextView) fragmentView.findViewById(R.id.tv_preview_desc);
            tvDescription.setText((amountToPay > 0
                    ? getActivity().getResources().getString(R.string.step_preview_desc)
                    : getActivity().getResources().getString(R.string.step_preview_desc_free)));
            //endregion

            //region set button text:
            Button btnConfirm = (Button) fragmentView.findViewById(R.id.btn_confirm);
            btnConfirm.setText(amountToPay > 0
                    ? getResources().getString(R.string.confirm_and_pay)
                    : getResources().getString(R.string.finish));
            //endregion

            //region set amount to pay text:
            TextView tvAmountToPay = (TextView) fragmentView.findViewById(R.id.tv_amount_to_pay);
            tvAmountToPay.setText(String.format(Locale.getDefault(), "%s: %,d",
                    getResources().getString(R.string.amount_to_pay), (int) amountToPay));
            tvAmountToPay.setVisibility((amountToPay > 0 ? View.VISIBLE : View.GONE));
            //endregion

            //region nav buttons:
            parentView.setPrevVisible(true);
            parentView.setOnPreviousClickListener(new Runnable() {
                @Override
                public void run() {
                    try {
                        parentView.showTemplateFieldsStep();
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });

            parentView.setNextVisible(false);
            //endregion

            //region hide keyboard:
            View focused = getActivity().getCurrentFocus();
            if (focused != null) {
                InputMethodManager imm = (InputMethodManager) getActivity().getSystemService(Context.INPUT_METHOD_SERVICE);
                imm.hideSoftInputFromWindow(focused.getWindowToken(), 0);
            }
            //endregion

            //region confirm button click:
            btnConfirm.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    try {
                        String trackingNumber = parentView.getCreatedConsolationTN();
                        String previewUrl =
                                String.format("http://www.samsys.ir/consolations/preview?tn=%s&ref=app",
                                trackingNumber);
                        Intent intent = new Intent(Intent.ACTION_VIEW);
                        intent.setData(Uri.parse(previewUrl));
                        startActivity(intent);
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });
            //endregion

        } catch (Exception ex) {
            ExceptionManager.handle(getActivity(), ex);
        }

        return fragmentView;
    }

    @Override
    public void onStart() {
        try {
            final String tn = parentView.getCreatedConsolationTN();
            final ProgressDialog progress = UxUtil.showProgress(getActivity());
            //region check payment status from api:
            new Thread(new Runnable() {
                @Override
                public void run() {
                    try {
                        ConsolationsApiEndpoint endpoint = ApiUtil.createEndpoint(ConsolationsApiEndpoint.class);
                        Response<ConsolationDto> response = endpoint.findByTrackingNumber(tn).execute();
                        if (!response.isSuccessful())
                            throw new CallServerException(getActivity());
                        ConsolationDto dto = response.body();
                        if (dto.getPaymentStatus().equals(PaymentStatus.verified.toString())) {
                            //region disable confirm button:
                            getActivity().runOnUiThread(new Runnable() {
                                @Override
                                public void run() {
                                    try {
                                        View v = getView();
                                        if (v != null) {
                                            Button btnConfirm = (Button) v.findViewById(R.id.btn_confirm);
                                            btnConfirm.setEnabled(false);
                                            btnConfirm.setText(getResources().getString(R.string.msg_successfully_payed));
                                        }
                                    } catch (Exception ex) {
                                        ExceptionManager.handle(getActivity(), ex);
                                    }
                                }
                            });
                            //endregion
                        }
                        //region hide progress:
                        getActivity().runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                progress.dismiss();
                            }
                        });
                        //endregion
                    } catch (Exception ex) {
                        //region hide progress:
                        getActivity().runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                progress.dismiss();
                            }
                        });
                        //endregion
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            }).start();
            //endregion
        } catch (Exception ex) {
            ExceptionManager.handle(getActivity(), ex);
        }

        super.onStart();
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
        URL url = new URL(new URL(Configs.API_BASE_ADDRESS), String.format("%s/%s?ts=%s", ApiActions.consolations_getpreview,
                Integer.toString(parentView.getCreatedConsolationId()),
                Long.toString(DateTimeUtility.getUTCNow().getTime())));
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