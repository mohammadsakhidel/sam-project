package com.ramanco.samandroid.fragments;


import android.app.ProgressDialog;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ImageButton;
import android.widget.ListView;
import android.widget.Toast;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.activities.MainActivity;
import com.ramanco.samandroid.adapters.ConsolationAdapter;
import com.ramanco.samandroid.api.dtos.ConsolationDto;
import com.ramanco.samandroid.api.endpoints.ConsolationsApiEndpoint;
import com.ramanco.samandroid.enums.PaymentStatus;
import com.ramanco.samandroid.exceptions.CallServerException;
import com.ramanco.samandroid.utils.ApiUtil;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.UxUtil;
import com.rey.material.widget.EditText;

import java.io.IOException;
import java.util.List;

import retrofit2.Response;

public class TrackFragment extends Fragment {

    //region Ctors:
    public TrackFragment() {
        // Required empty public constructor
    }
    //endregion

    //region Overrides:
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        final View fragmentView = inflater.inflate(R.layout.fragment_track, container, false);

        try {

            //region search button click:
            ImageButton btnSearch = (ImageButton) fragmentView.findViewById(R.id.btn_search);
            btnSearch.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    try {
                        EditText etTrackingNumber = (EditText) fragmentView.findViewById(R.id.et_tracking_number);
                        String trackingNumber = etTrackingNumber.getText().toString();
                        if (!TextUtils.isEmpty(trackingNumber.replace(" ", ""))) {
                            findConsolationAsync(trackingNumber);
                        } else {
                            Toast.makeText(getActivity(),
                                    getActivity().getResources().getString(R.string.msg_invalid_tracking_number),
                                    Toast.LENGTH_SHORT).show();
                        }
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
    //endregion

    //region Methods:
    private void findConsolationAsync(final String trackNumber) throws IOException, CallServerException {

        //region call server and fill list view:
        final ProgressDialog progress = UxUtil.showProgress(getActivity());
        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    ConsolationsApiEndpoint endpoint = ApiUtil.createEndpoint(ConsolationsApiEndpoint.class);
                    String[] trackNumbersArray = new String[]{trackNumber};
                    Response<ConsolationDto[]> response = endpoint.find(trackNumbersArray).execute();
                    if (!response.isSuccessful())
                        throw new CallServerException(getActivity());
                    final ConsolationDto[] consolations = response.body();
                    if (consolations.length > 0) {
                        //region fill list view:
                        getActivity().runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                try {
                                    View fragmentView = getView();
                                    if (fragmentView != null) {
                                        ConsolationAdapter adapter = new ConsolationAdapter(getActivity(), consolations);
                                        ListView lvItems = (ListView) fragmentView.findViewById(R.id.lv_items);
                                        lvItems.setAdapter(adapter);
                                        progress.dismiss();
                                    }
                                } catch (Exception ex) {
                                    progress.dismiss();
                                    ExceptionManager.handle(getActivity(), ex);
                                }
                            }
                        });
                        //endregion
                    } else {
                        getActivity().runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                try {
                                    progress.dismiss();
                                    Toast.makeText(getActivity(),
                                            getActivity().getResources().getString(R.string.msg_no_result_found),
                                            Toast.LENGTH_SHORT).show();
                                } catch (Exception ex) {
                                    ExceptionManager.handle(getActivity(), ex);
                                }
                            }
                        });
                    }
                } catch (Exception ex) {
                    progress.dismiss();
                    ExceptionManager.handle(getActivity(), ex);
                }
            }
        }).start();
        //endregion

    }
    //endregion

}
