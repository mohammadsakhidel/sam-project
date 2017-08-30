package com.ramanco.samandroid.fragments;


import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.ImageView;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.utils.ExceptionManager;
import java.io.File;
import uk.co.senab.photoview.PhotoViewAttacher;

public class ImageViewerFragment extends Fragment {

    //region Args:
    public static final String ARG_IMAGE_PATH = "image_path";
    public static final String ARG_IMAGE_BITMAP = "image_bitmap";
    //endregion

    //region Ctors:
    public ImageViewerFragment() {
    }
    //endregion

    //region Fields:
    PhotoViewAttacher photoViewAttacher;
    Bitmap bitmap;
    //endregion

    //region Getters & Setters:
    public Bitmap getBitmap() {
        return bitmap;
    }

    public void setBitmap(Bitmap bitmap) {
        this.bitmap = bitmap;
    }
    //endregion

    //region Implementation:
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        View view = inflater.inflate(R.layout.fragment_image_viewer, container, false);

        try {

            //region set image:
            String imagePath = getArguments() != null && getArguments().containsKey(ARG_IMAGE_PATH)
                    ? getArguments().getString(ARG_IMAGE_PATH) : "";
            if (!TextUtils.isEmpty(imagePath)) {
                File imageFile = new File(imagePath);
                if (imageFile.exists()) {
                    ImageView imageView = (ImageView) view.findViewById(R.id.img_picture);
                    imageView.setImageBitmap(BitmapFactory.decodeFile(imageFile.getPath()));
                    photoViewAttacher = new PhotoViewAttacher(imageView, true);
                }
            } else if (getBitmap() != null) {
                ImageView imageView = (ImageView) view.findViewById(R.id.img_picture);
                imageView.setImageBitmap(getBitmap());
                photoViewAttacher = new PhotoViewAttacher(imageView, true);
            }
            //endregion
            //region back button:
            ImageButton btnBack = (ImageButton) view.findViewById(R.id.btn_back);
            btnBack.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    try {
                        FragmentManager fm = getActivity().getSupportFragmentManager();
                        int stackCount = fm.getBackStackEntryCount();
                        if (stackCount > 0)
                            fm.popBackStack();
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });
            //endregion

        } catch (Exception ex) {
            ExceptionManager.handle(getActivity(), ex);
        }

        return view;

    }
    //endregion

}
